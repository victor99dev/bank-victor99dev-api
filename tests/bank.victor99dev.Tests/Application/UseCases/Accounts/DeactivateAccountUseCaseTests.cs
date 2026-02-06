using bank.victor99dev.Application.Shared.Messaging;
using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.DeactivateAccount;
using bank.victor99dev.Application.UseCases.Accounts.CreateAccount;
using bank.victor99dev.Domain.Exceptions;
using bank.victor99dev.Tests.Application.Shared;
using bank.victor99dev.Tests.Infrastructure.Shared;

namespace bank.victor99dev.Tests.Application.UseCases.Accounts;

public class DeactivateAccountUseCaseTests
{
    [Fact(DisplayName = "Should deactivate, enqueue event and invalidate cache when cache seeded")]
    public async Task ShouldDeactivateAccount_WithCache_EnqueueEvent_InvalidateCache()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var cache = new FakeAccountCacheRepository();
        var dispatcher = new FakeDomainEventDispatcher();
        var factory = new AccountEventFactory();

        var request = AccountRequests.Valid(seed: 1);
        var created = await new CreateAccountUseCase(repo, uow, cache, factory, dispatcher).ExecuteAsync(request);
        var id = created.Data!.Id;

        cache.Seed(new()
        {
            Id = id,
            Name = created.Data!.Name,
            Cpf = request.Cpf,
            IsActive = true,
            IsDeleted = false,
            CreatedAt = created.Data!.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        });

        var before = dispatcher.Enqueued.Count;

        var useCase = new DeactivateAccountUseCase(repo, uow, cache, dispatcher, factory);
        var result = await useCase.ExecuteAsync(id);

        Assert.True(result.IsSuccess);

        Assert.Equal(before + 1, dispatcher.Enqueued.Count);
        Assert.Contains(dispatcher.Enqueued, e => e.GetType().Name == "AccountDeactivatedDomainEvent");

        Assert.True(cache.InvalidateCalls is 1 or 2);
        Assert.False(cache.ContainsId(id));
        Assert.False(cache.ContainsCpf(request.Cpf));
    }


    [Fact(DisplayName = "Should throw DomainException when deactivating already inactive account and not enqueue event nor invalidate cache")]
    public async Task ShouldThrow_WhenAlreadyInactive_NoEvent_NoInvalidate()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var cache = new FakeAccountCacheRepository();
        var dispatcher = new FakeDomainEventDispatcher();
        var factory = new AccountEventFactory();

        var request = AccountRequests.Valid(seed: 1);
        var created = await new CreateAccountUseCase(repo, uow, cache, factory, dispatcher).ExecuteAsync(request);
        var id = created.Data!.Id;

        var uc1 = new DeactivateAccountUseCase(repo, uow, cache, dispatcher, factory);
        var first = await uc1.ExecuteAsync(id);
        Assert.True(first.IsSuccess);

        var eventsBefore = dispatcher.Enqueued.Count;
        var invalidatesBefore = cache.InvalidateCalls;

        var uc2 = new DeactivateAccountUseCase(repo, uow, cache, dispatcher, factory);

        await Assert.ThrowsAsync<DomainException>(() => uc2.ExecuteAsync(id));

        Assert.Equal(eventsBefore, dispatcher.Enqueued.Count);
        Assert.Equal(invalidatesBefore, cache.InvalidateCalls);
    }

    [Fact(DisplayName = "Should return NotFound and not enqueue event nor invalidate cache when account does not exist")]
    public async Task ShouldReturnNotFound_WhenAccountDoesNotExist_NoEvent_NoInvalidate()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var cache = new FakeAccountCacheRepository();
        var dispatcher = new FakeDomainEventDispatcher();
        var factory = new AccountEventFactory();

        var useCase = new DeactivateAccountUseCase(repo, uow, cache, dispatcher, factory);
        var result = await useCase.ExecuteAsync(Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);

        Assert.Empty(dispatcher.Enqueued);
        Assert.Equal(0, cache.InvalidateCalls);
    }
}