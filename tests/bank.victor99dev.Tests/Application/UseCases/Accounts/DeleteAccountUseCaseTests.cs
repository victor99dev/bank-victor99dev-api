using bank.victor99dev.Application.Shared.Messaging;
using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.DeleteAccount;
using bank.victor99dev.Application.UseCases.Accounts.CreateAccount;
using bank.victor99dev.Tests.Application.Shared;
using bank.victor99dev.Tests.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;
using bank.victor99dev.Domain.Entities;

namespace bank.victor99dev.Tests.Application.UseCases.Accounts;

public class DeleteAccountUseCaseTests
{
    [Fact(DisplayName = "Should soft delete, enqueue events and invalidate cache when cache seeded")]
    public async Task ShouldSoftDeleteAccount_WithCache_EnqueueEvents_InvalidateCache()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (ctx, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

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

        var useCase = new DeleteAccountUseCase(repo, uow, cache, dispatcher, factory);
        var result = await useCase.ExecuteAsync(id);

        Assert.True(result.IsSuccess);

        var raw = await ctx.Set<Account>().FirstAsync(x => x.Id == id);
        Assert.True(raw.IsDeleted);
        Assert.False(raw.IsActive);
        Assert.NotNull(raw.UpdatedAt);

        Assert.Equal(before + 2, dispatcher.Enqueued.Count);
        Assert.Contains(dispatcher.Enqueued, e => e.GetType().Name == "AccountDeletedDomainEvent");
        Assert.Contains(dispatcher.Enqueued, e => e.GetType().Name == "AccountUpdatedDomainEvent");

        Assert.True(cache.InvalidateCalls is 1 or 2);
        Assert.False(cache.ContainsId(id));
        Assert.False(cache.ContainsCpf(request.Cpf));
    }

    [Fact(DisplayName = "Should soft delete, enqueue events and invalidate cache even when cache empty")]
    public async Task ShouldSoftDeleteAccount_NoCache_EnqueueEvents_InvalidateCache()
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

        var useCase = new DeleteAccountUseCase(repo, uow, cache, dispatcher, factory);
        var result = await useCase.ExecuteAsync(id);

        Assert.True(result.IsSuccess);

        Assert.Equal(before + 2, dispatcher.Enqueued.Count);
        Assert.Contains(dispatcher.Enqueued, e => e.GetType().Name == "AccountDeletedDomainEvent");
        Assert.Contains(dispatcher.Enqueued, e => e.GetType().Name == "AccountUpdatedDomainEvent");

        Assert.True(cache.InvalidateCalls is 1 or 2);
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

        var useCase = new DeleteAccountUseCase(repo, uow, cache, dispatcher, factory);
        var result = await useCase.ExecuteAsync(Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);

        Assert.Empty(dispatcher.Enqueued);
        Assert.Equal(0, cache.InvalidateCalls);
    }
}