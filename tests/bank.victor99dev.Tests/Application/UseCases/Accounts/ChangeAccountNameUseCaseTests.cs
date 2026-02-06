using bank.victor99dev.Application.Shared.Messaging;
using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.ChangeAccountName;
using bank.victor99dev.Application.UseCases.Accounts.CreateAccount;
using bank.victor99dev.Tests.Application.Shared;
using bank.victor99dev.Tests.Infrastructure.Shared;

namespace bank.victor99dev.Tests.Application.UseCases.Accounts;

public class ChangeAccountNameUseCaseTests
{
    [Fact(DisplayName = "Should change name, enqueue events and invalidate cache when cache seeded")]
    public async Task ShouldChangeName_WithCache_EnqueueEvents_InvalidateCache()
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

        var useCase = new ChangeAccountNameUseCase(repo, uow, cache, dispatcher, factory);
        var result = await useCase.ExecuteAsync(id, new ChangeAccountNameRequest { Name = "New Name" });

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);

        Assert.Equal(id, result.Data!.Id);
        Assert.Equal("New Name", result.Data.Name);
        Assert.Equal(request.Cpf, result.Data.Cpf);

        Assert.Equal(before + 2, dispatcher.Enqueued.Count);
        Assert.Contains(dispatcher.Enqueued, e => e.GetType().Name == "AccountNameChangedDomainEvent");
        Assert.Contains(dispatcher.Enqueued, e => e.GetType().Name == "AccountUpdatedDomainEvent");

        Assert.True(cache.InvalidateCalls is 1 or 2);
        Assert.False(cache.ContainsId(id));
        Assert.False(cache.ContainsCpf(request.Cpf));
    }

    [Fact(DisplayName = "Should change name, enqueue events and invalidate cache even when cache empty")]
    public async Task ShouldChangeName_NoCache_EnqueueEvents_InvalidateCache()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var cache = new FakeAccountCacheRepository();
        var dispatcher = new FakeDomainEventDispatcher();
        var factory = new AccountEventFactory();

        var request = AccountRequests.Valid(seed: 1);
        var created = await new CreateAccountUseCase(repo, uow, cache, factory, dispatcher).ExecuteAsync(request);
        var id = created.Data!.Id;

        var before = dispatcher.Enqueued.Count;
        var invalidateBefore = cache.InvalidateCalls;

        var useCase = new ChangeAccountNameUseCase(repo, uow, cache, dispatcher, factory);
        var result = await useCase.ExecuteAsync(id, new ChangeAccountNameRequest { Name = "New Name" });

        Assert.True(result.IsSuccess);

        Assert.Equal(before + 2, dispatcher.Enqueued.Count);
        Assert.Contains(dispatcher.Enqueued, e => e.GetType().Name == "AccountNameChangedDomainEvent");
        Assert.Contains(dispatcher.Enqueued, e => e.GetType().Name == "AccountUpdatedDomainEvent");

        Assert.True(cache.InvalidateCalls is 1 or 2);
        Assert.Equal(invalidateBefore + cache.InvalidateCalls - invalidateBefore, cache.InvalidateCalls);

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

        var useCase = new ChangeAccountNameUseCase(repo, uow, cache, dispatcher, factory);
        var result = await useCase.ExecuteAsync(Guid.NewGuid(), new ChangeAccountNameRequest { Name = "New Name" });

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);

        Assert.Empty(dispatcher.Enqueued);
        Assert.Equal(0, cache.InvalidateCalls);
    }
}