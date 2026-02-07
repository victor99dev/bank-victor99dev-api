# Accounts & Events API

## Building an Event-Driven Accounts API

##### [ 🇧🇷 Leia esta página em Português | Read this page in Português](./docs/README-pt.md)

This project implements an **Accounts API** inspired by real-world scenarios of modern, high-criticality banking systems.

It was conceived as **a technical assessment for a bank**, adopting practices and patterns that usually only appear in production corporate systems, where reliability, consistency, observability, and decoupling between domains are fundamental requirements.

More than a simple CRUD, the project seeks to demonstrate in practice:

* How to model a **rich domain with DDD (Domain-Driven Design)**,
* How to ensure **reliable asynchronous communication using the Outbox Pattern**,
* How to reduce operational cost and latency with **intelligent caching in Redis**,
* How to promote **decoupling between business areas via Kafka and event-driven architecture**,
* And how to organize all of this within a **Clean Architecture aligned with SOLID principles**.

In practice, this service represents the **core of a banking accounts system**, acting as the source of truth for the *Account* domain and emitting events that can be consumed by several areas of the  organization, such as:

* **Fraud** - risk monitoring and prevention,
* **Cards** - status updates and relationship with accounts,
* **CRM** - centralized customer view,
* **Notifications** - proactive communication with users,
* **Open Finance** - secure data sharing,
* **Analytics** - construction of metrics and insights,
* **Backoffice** - administrative operations and auditing.

The design directly addresses the two central problems of the technical assessment:

1)  **Integration between bank areas**: multiple domains react to changes in the account lifecycle through asynchronous events.

2)  **Reduction of cost with repeated database queries**: use of Redis as a distributed cache with daily TTL and consistent post-commit invalidation.

All these areas can **react to the events published by this service**, without direct coupling, ensuring team autonomy and ecosystem scalability.

### Key technical pillars

* **Clean Architecture + SOLID**
* **DDD with explicit Domain Events**
* **Transactional Outbox Pattern for reliability**
* **Kafka as the event backbone between areas**
* **Redis with daily cache and consistent invalidation**
* **EF Core + PostgreSQL as main persistence**
* **Basic observability (structured logs) and well-defined dependency injection**

---

## Technologies

* **.NET 8**
* **Entity Framework Core 8**
* **PostgreSQL (Npgsql)**
* **Confluent.Kafka**
* **Redis (StackExchange.Redis)**
* **Swagger / OpenAPI**
* **xUnit**

---

## Overall Architecture

![img 1](/docs/img/img1.png)

### Diagram summary

This diagram presents the **layered structural view of the solution**, based on Clean Architecture and the principle of separation of responsibilities.

**Domain**
Contains the business core and represents the conceptual source of truth of the system.
It includes the `Account` aggregate, the `Cpf` Value Object, and **explicit Domain Events**, which materialize relevant business facts.
The `IHasAggregateKey` interface ensures that every event carries a stable partition key for Kafka, preserving order and coherence per account.

**Application**
Acts as the orchestrator of use cases, coordinating business rules, repositories, Unit of Work, and cache.
The `DomainEventDispatcher` transforms domain events into Outbox items in a standardized way, decoupling the domain from any messaging technology.
The layer defines **ports (interfaces)** for persistence, cache, and messaging, maintaining inverted dependencies.

**Infrastructure**
Concentrates all external integrations:
- EF Core + PostgreSQL for transactional persistence,
- Redis as distributed cache shared between instances,
- Kafka Producer for event publication,
- **OutboxProcessorWorker** responsible for ensuring resilient and asynchronous delivery of events.

The set ensures that the domain remains pure, while technical decisions are isolated in the infrastructure.

---

## Write Flow

![img 2](/docs/img/img2.png)

This diagram details the **critical write path** and how the system guarantees consistency between database and events.

In any operation that changes the state of `Account`:

1.  The Use Case loads the aggregate when necessary.
2.  The mutation is applied within the domain, respecting business invariants.
3.  A **Domain Event** is created to represent the fact that occurred.
4.  The `DomainEventDispatcher` serializes the event and stores a record in the `outbox_messages` table.
5.  `SaveChangesAsync()` persists **Account and Outbox in the same atomic transaction**, avoiding inconsistencies.
6.  Only after a successful commit is the Redis cache invalidated to prevent stale data reads.
7.  The Redis cache is invalidated after commit.

Invalidation strategy: - `accounts:id:{id}`
- `accounts:cpf:{newCpf}`
- `accounts:cpf:{oldCpf}` *(when there is a CPF change)*

### Cache keys:

    accounts:id:{accountId}
    accounts:cpf:{cpf}

Whenever there is a change in the `Account` aggregate, the corresponding cache is **invalidated immediately after the transactional commit**.

In the specific case of CPF change, **two entries are invalidated**:

* the key associated with the **old CPF**.
* the key associated with the **new CPF**.

This prevents any possibility of reading inconsistent or divergent data between database and cache.

On the next read request, the data is materialized again in Redis from PostgreSQL and stored with **daily TTL**, ensuring that the cache is always synchronized with the source of truth, but without generating unnecessary load on the database throughout the day.

---

## End-to-End Flow

![img 3](/docs/img/img3.png)

This diagram illustrates the **complete event cycle**, clearly separating synchronous and asynchronous responsibility.

### Synchronous layer (fast and predictable)

-   The HTTP request is processed by the API.
-   Data and Outbox are written to the database in a single transaction.
-   The API returns success immediately to the client.

### Asynchronous layer (reliable and resilient)
The **OutboxProcessorWorker** periodically executes:

1.  Fetches unpublished events from `outbox_messages`.
2.  Publishes to the topic `bank.account.events.v1`.
3.  On success, marks the event as published.
4.  On failure, logs the error and automatically retries.

Other areas consume the topic and react to the events, without direct dependency on this service, characterizing a truly event-driven architecture.

---

## Asynchronous Flow

The **OutboxProcessorWorker** periodically executes:

1.  Fetches unpublished events;
2.  Publishes to the topic `bank.account.events.v1`;
3.  Marks as published on success;
4.  Performs automatic retries on failure.

Kafka uses **Key = AccountId**, guaranteeing order per aggregate.

---

## Data Model

![img 4](/docs/img/img4.png)

**accounts**
Is the **source of truth of the domain**, containing the central account data.

**outbox_messages**
Implements the Outbox pattern with fields such as event type, serialized payload, timestamp, number of retries, and errors, allowing reprocessing, auditing, and traceability.

**Redis (logical cache)**
Stores frequent responses with daily TTL:
```
accounts:id:{accountId}
accounts:cpf:{cpf}
```

Reduces read cost from the database and improves performance without compromising consistency.

---

## Emitted Domain Events

This service acts as the **canonical producer of events of the `Account` aggregate**, materializing relevant domain changes through **explicit Domain Events**. These events represent completed business facts and are reliably published via **Outbox + Kafka**, guaranteeing order, traceability, and reprocessing when necessary.

Emitted events:

* `AccountCreatedDomainEvent`
* `AccountDeactivatedDomainEvent`
* `AccountActivatedDomainEvent`
* `AccountDeletedDomainEvent`
* `AccountRestoredDomainEvent`
* `AccountUpdatedDomainEvent`
* `AccountNameChangedDomainEvent`
* `AccountCpfChangedDomainEvent`

Each event carries the **aggregation key (`AccountId`) as Kafka Key**, preserving ordering per account and allowing consumers to rebuild state or create consistent projections.

### Consumption by downstream domains

Different bank areas **react asynchronously and decoupled** to these
events, for example:

* **Fraud:** initiates or adjusts risk monitoring;
* **Cards:** blocks, suspends, or enables card issuance;
* **CRM:** keeps the centralized and updated customer view;
* **Notifications:** triggers proactive communication to the user;
* **Analytics:** feeds metrics, indicators, and analytical dashboards.

None of these integrations occur within the HTTP request, all systemic effects are processed **outside the critical path**, via Kafka.

### Practical evidence of operation

The screenshots below demonstrate, in practice, that the events are effectively produced and that other areas react automatically to them (Fraud, Cards, CRM, and Analytics), confirming the end-to-end flow of the event-driven architecture:

> **Security and privacy note:**
> The CPFs used in the examples are **not real personal data**. They were generated exclusively for testing and technical demonstration using the public tool:
> [4devs.com](https://www.4devs.com.br/gerador_de_cpf)
>
> This avoids any exposure of sensitive personal data and keeps the project aligned with good data protection practices.

<div align="center">
  <img src="/docs/img/events/img1.png" width="32%" style="margin: 0 6px;" />
  <img src="/docs/img/events/img2.png" width="37%" style="margin: 0 6px;" />
</div>

<div align="center">
  <img src="/docs/img/events/img3.png" width="32%" style="margin: 0 6px;" />
  <img src="/docs/img/events/img4.png" width="33%" style="margin: 0 6px;" />
</div>

---

### Validation Consumer

In addition to the service logs and the evidence above, I made available a separate repository used to **test the consumption of events published in Kafka**.
It works as a **reference consumer**, ensuring that the events emitted by the topic can be read, deserialized, and processed by external services (simulating downstream domains).

Repository:

* [producer-and-consumer-kafka-message](https://github.com/victor99dev/producer-and-consumer-kafka-message)

This consumer was used to validate:

* Connectivity with the Kafka broker;
* Continuous topic consumption;
* Reading by **partition** with **Key = AccountId** (order per aggregate);
* Payload deserialization and inspection of event content;
* Practical confirmation that the ecosystem can react to events without direct dependency on the API.

------------------------------------------------------------------------

## Commands
| Command | Description |
|--------|-------------|
| `dotnet clean` | Removes build artifacts (bin/ and obj/) |
| `dotnet restore` | Restores NuGet packages |
| `dotnet build` | Builds the solution |
| `dotnet test` | Runs all automated tests |
| `dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura` | Runs tests and generates XML coverage report |
| `dotnet run --project src/bank.victor99dev.Api` | Starts the API locally |
| `dotnet ef migrations add NomeMigracao -p src/bank.victor99dev.Infrastructure -s src/bank.victor99dev.Api` | Creates a migration |
| `dotnet ef database update -p src/bank.victor99dev.Infrastructure -s src/bank.victor99dev.Api` | Applies migrations |
| `docker compose up -d --build` | Runs API + Postgres + Redis + Kafka |
| `docker compose stop` | Stops all containers (keeps data) |
| `docker compose down` | Stops and removes containers (keeps volumes) |
| `docker compose down -v` | Stops and removes containers and deletes volumes |


> To test the application with all dependencies, simply run: `docker compose up -d --build`.

------------------------------------------------------------------------

## Accessing the API

-   Swagger: `http://localhost:{PORT}/swagger`
-   Use Postman or Insomnia

------------------------------------------------------------------------

## Endpoints

> Base URL: `/api/accounts`

<details><summary><b>Accounts Endpoints</b></summary>
<p>

| Method | Endpoint | Parameter | Body |
|------:|----------|-----------|------|
| POST | `/api/accounts` | N/A | `CreateAccountRequest` |
| GET | `/api/accounts/{accountId}` | `accountId` | N/A |
| GET | `/api/accounts` | query | N/A |
| GET | `/api/accounts/{cpf}/cpf` | `cpf` | N/A |
| PUT | `/api/accounts/{accountId}` | `accountId` | `UpdateAccountRequest` |
| PATCH | `/api/accounts/{accountId}/name` | `accountId` | `ChangeAccountNameRequest` |
| PATCH | `/api/accounts/{accountId}/cpf` | `accountId` | `ChangeAccountCpfRequest` |
| PATCH | `/api/accounts/{accountId}/activate` | `accountId` | N/A |
| PATCH | `/api/accounts/{accountId}/deactivate` | `accountId` | N/A |
| PATCH | `/api/accounts/{accountId}/restore` | `accountId` | N/A |
| DELETE | `/api/accounts/{accountId}` | `accountId` | N/A |
</p>
</details>

### Payload examples

<details><summary><b>CreateAccountRequest</b></summary>
<p>

```json
{
  "name": "Victor Hugo",
  "cpf": "12345678901"
}
```
</p>
</details>

<details><summary><b>UpdateAccountRequest</b></summary>
<p>

```json
{
  "name": "Victor Hugo",
  "cpf": "12345678901",
  "isActive": true,
  "isDeleted": false
}
```
</p>
</details>

<details><summary><b>ChangeAccountNameRequest</b></summary>
<p>

```json
{
  "name": "Novo Nome"
}
```
</p>
</details>

<details><summary><b>ChangeAccountCpfRequest</b></summary>
<p>

```json
{
  "cpf": "10987654321"
}
```
</p>
</details>

The endpoints cover the entire lifecycle of the **Account** aggregate, including creation, reading, data update, and state changes.

---

## Project Structure

```
src/
  bank.victor99dev.Api
  bank.victor99dev.Application
  bank.victor99dev.Domain
  bank.victor99dev.Infrastructure
tests/
  bank.victor99dev.Tests.Application
  bank.victor99dev.Tests.Domain
  bank.victor99dev.Tests.Infrastructure
```

---

## What this project demonstrates

* **Complete CRUD with thin controllers**, acting only as HTTP adapters;
* **DDD applied concretely**, with `Account` aggregate, `Cpf` Value Object, and validations/invariants inside the domain;
* **Pure Domain Events**, without infrastructure dependency in the Domain layer;
* **Transactional Outbox Pattern**, ensuring atomic consistency between **database state and event record**;
* **Resilient asynchronous event delivery** via `OutboxProcessorWorker`, with retry, operational idempotence, and traceability;
* **Kafka as integration backbone**, using `Key = AccountId` to preserve ordering per aggregate and avoid race conditions among consumers;
* **Decoupling between business domains**, allowing independent evolution of Fraud, Cards, CRM, Notifications, and Analytics;
* **Redis as distributed cache with daily TTL**, reducing cloud cost and read latency;
* **Deterministic post-commit cache invalidation**, including invalidation of old and new CPF keys in registration changes;
* **Clear separation between transactional consistency (database + outbox) and eventual consistency (downstream events)**;
* **Architecture prepared for organizational scale**, not only technical scale.

> The project demonstrates how to transform a simple CRUD into a  **canonical event-driven service**, capable of operating as the source of truth in a real banking microservices ecosystem, balancing consistency, cost, and decoupling.

---

## My Links

* GitHub: https://github.com/torugo99
* LinkedIn: https://www.linkedin.com/in/victor-hugo99
* Website: https://victor99dev.website

---

### Credits and Acknowledgments

* Official .NET documentation: https://learn.microsoft.com/dotnet
* Swagger / OpenAPI: https://learn.microsoft.com/aspnet/core/tutorials/getting-started-with-swashbuckle
* PostgreSQL: https://www.postgresql.org
* Redis: https://redis.io
* Kafka concepts: https://kafka.apache.org/documentation
