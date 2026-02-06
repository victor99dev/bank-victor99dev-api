using bank.victor99dev.Application.Shared.Messaging;
using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.ActivateAccount;
using bank.victor99dev.Application.UseCases.Accounts.CreateAccount;
using bank.victor99dev.Application.UseCases.Accounts.DeactivateAccount;
using bank.victor99dev.Domain.Entities;
using bank.victor99dev.Tests.Application.Shared;
using bank.victor99dev.Tests.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace bank.victor99dev.Tests.Application.UseCases.Accounts;

public class ActivateAccountUseCaseTests
{
    [Fact(DisplayName = "Should activate an inactive account, enqueue event and invalidate cache when cache seeded")]
    public async Task ShouldActivateAccount_WithCache_EnqueueEvent_InvalidateCache()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (ctxA, repoA, uowA) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var cache = new FakeAccountCacheRepository();
        var dispatcher = new FakeDomainEventDispatcher();
        var factory = new AccountEventFactory();

        var request = AccountRequests.Valid(seed: 1);
        var created = await new CreateAccountUseCase(repoA, uowA, cache, factory, dispatcher).ExecuteAsync(request);
        var id = created.Data!.Id;

        var (_, repoB, uowB) = EntityFrameworkInMemoryFactory.CreateInfra(db);
        var deactivated = await new DeactivateAccountUseCase(repoB, uowB, cache, dispatcher, factory).ExecuteAsync(id);
        Assert.True(deactivated.IsSuccess);

        cache.Seed(new()
        {
            Id = id,
            Name = created.Data!.Name,
            Cpf = request.Cpf,
            IsActive = false,
            IsDeleted = false,
            CreatedAt = created.Data!.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        });

        var eventsBefore = dispatcher.Enqueued.Count;
        var invalidatesBefore = cache.InvalidateCalls;

        var (ctxC, repoC, uowC) = EntityFrameworkInMemoryFactory.CreateInfra(db);
        var useCase = new ActivateAccountUseCase(repoC, uowC, cache, dispatcher, factory);
        var result = await useCase.ExecuteAsync(id);

        Assert.True(result.IsSuccess);

        var raw = await ctxC.Set<Account>().FirstAsync(x => x.Id == id);
        Assert.True(raw.IsActive);
        Assert.False(raw.IsDeleted);
        Assert.NotNull(raw.UpdatedAt);

        Assert.Equal(eventsBefore + 1, dispatcher.Enqueued.Count);
        Assert.Equal("AccountActivatedDomainEvent", dispatcher.Enqueued.Last().GetType().Name);

        Assert.True(cache.InvalidateCalls >= invalidatesBefore + 1);
        Assert.False(cache.ContainsId(id));
        Assert.False(cache.ContainsCpf(request.Cpf));
    }

    [Fact(DisplayName = "Should activate an inactive account, enqueue event and invalidate cache even when cache empty")]
    public async Task ShouldActivateAccount_NoCache_EnqueueEvent_InvalidateCache()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (ctxA, repoA, uowA) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var cache = new FakeAccountCacheRepository();
        var dispatcher = new FakeDomainEventDispatcher();
        var factory = new AccountEventFactory();

        var request = AccountRequests.Valid(seed: 1);
        var created = await new CreateAccountUseCase(repoA, uowA, cache, factory, dispatcher).ExecuteAsync(request);
        var id = created.Data!.Id;

        var (ctxB, repoB, uowB) = EntityFrameworkInMemoryFactory.CreateInfra(db);
        var deactivated = await new DeactivateAccountUseCase(repoB, uowB, cache, dispatcher, factory).ExecuteAsync(id);
        Assert.True(deactivated.IsSuccess);

        var invalidateBefore = cache.InvalidateCalls;
        var eventsBefore = dispatcher.Enqueued.Count;

        var (ctxC, repoC, uowC) = EntityFrameworkInMemoryFactory.CreateInfra(db);
        var useCase = new ActivateAccountUseCase(repoC, uowC, cache, dispatcher, factory);
        var result = await useCase.ExecuteAsync(id);

        Assert.True(result.IsSuccess);

        var raw = await ctxC.Set<Account>().FirstAsync(x => x.Id == id);
        Assert.True(raw.IsActive);
        Assert.False(raw.IsDeleted);
        Assert.NotNull(raw.UpdatedAt);

        Assert.Equal(eventsBefore + 1, dispatcher.Enqueued.Count);
        Assert.Contains(dispatcher.Enqueued, e => e.GetType().Name == "AccountActivatedDomainEvent");

        Assert.Equal(invalidateBefore + 1, cache.InvalidateCalls);
        Assert.False(cache.ContainsId(id));
        Assert.False(cache.ContainsCpf(request.Cpf));
    }

    [Fact(DisplayName = "Should return NotFound and not enqueue event nor invalidate cache when account does not exist")]
    public async Task ShouldReturnNotFound_WhenAccountDoesNotExist_NoEvent_NoInvalidate()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var cache = new FakeAccountCacheRepository();
        var dispatcher = new FakeDomainEventDispatcher();
        var factory = new AccountEventFactory();

        var useCase = new ActivateAccountUseCase(repo, uow, cache, dispatcher, factory);
        var result = await useCase.ExecuteAsync(Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);

        Assert.Empty(dispatcher.Enqueued);
        Assert.Equal(0, cache.InvalidateCalls);
    }
}