using bank.victor99dev.Application.Interfaces.Messaging;
using bank.victor99dev.Domain.Entities;
using bank.victor99dev.Domain.Events;
using bank.victor99dev.Domain.Events.Bodies;
using bank.victor99dev.Domain.Primitives;

namespace bank.victor99dev.Application.Shared.Messaging;

public sealed class AccountEventFactory : IAccountEventFactory
{
    private static Guid NewId() => Guid.NewGuid();
    private static DateTimeOffset Now() => DateTimeOffset.UtcNow;

    public IDomainEvent Created(Account account)
    {
        return new AccountCreatedDomainEvent
        {
            EventId = NewId(),
            OccurredOnUtc = Now(),
            Body = new AccountCreatedBody
            {
                AccountId = account.Id,
                Cpf = account.Cpf.Value,
                Name = account.AccountName.Value,
                IsActive = account.IsActive,
                IsDeleted = account.IsDeleted,
                CreatedAt = account.CreatedAt
            }
        };
    }

    public IDomainEvent Activated(Account account)
    {
        return new AccountActivatedDomainEvent
        {
            EventId = NewId(),
            OccurredOnUtc = Now(),
            AccountId = account.Id
        };
    }

    public IDomainEvent Deactivated(Account account)
    {
        return new AccountDeactivatedDomainEvent
        {
            EventId = NewId(),
            OccurredOnUtc = Now(),
            AccountId = account.Id
        };
    }

    public IDomainEvent Deleted(Account account)
    {
        return new AccountDeletedDomainEvent
        {
            EventId = NewId(),
            OccurredOnUtc = Now(),
            AccountId = account.Id
        };
    }

    public IDomainEvent Restored(Account account)
    {
        return new AccountRestoredDomainEvent
        {
            EventId = NewId(),
            OccurredOnUtc = Now(),
            AccountId = account.Id
        };
    }

    public IDomainEvent CpfChanged(Account account, string oldCpf)
    {
        return new AccountCpfChangedDomainEvent
        {
            EventId = NewId(),
            OccurredOnUtc = Now(),
            Body = new AccountCpfChangedBody
            {
                AccountId = account.Id,
                OldCpf = oldCpf,
                NewCpf = account.Cpf.Value
            }
        };
    }

    public IDomainEvent NameChanged(Account account, string oldName)
    {
        return new AccountNameChangedDomainEvent
        {
            EventId = NewId(),
            OccurredOnUtc = Now(),
            Body = new AccountNameChangedBody
            {
                AccountId = account.Id,
                OldName = oldName,
                NewName = account.AccountName.Value
            }
        };
    }

    public IDomainEvent Updated(Account account)
    {
        var updatedAt = account.UpdatedAt!.Value;
        var updatedAtOffset = new DateTimeOffset(updatedAt, TimeSpan.Zero);

        return new AccountUpdatedDomainEvent
        {
            EventId = NewId(),
            OccurredOnUtc = Now(),
            Body = new AccountUpdatedBody
            {
                AccountId = account.Id,
                Name = account.AccountName.Value,
                Cpf = account.Cpf.Value,
                IsActive = account.IsActive,
                IsDeleted = account.IsDeleted,
                UpdatedAt = updatedAtOffset
            }
        };
    }
}
