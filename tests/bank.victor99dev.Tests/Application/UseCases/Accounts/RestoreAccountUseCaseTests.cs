using bank.victor99dev.Application.Shared.Messaging;
using bank.victor99dev.Application.UseCases.Accounts.CreateAccount;
using bank.victor99dev.Application.UseCases.Accounts.DeleteAccount;
using bank.victor99dev.Application.UseCases.Accounts.RestoreAccount;
using bank.victor99dev.Domain.Entities;
using bank.victor99dev.Tests.Application.Shared;
using bank.victor99dev.Tests.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace bank.victor99dev.Tests.Application.UseCases.Accounts;

public class RestoreAccountUseCaseTests
{
    [Fact(DisplayName = "Should restore a deleted account and invalidate cache")]
    public async Task ShouldRestoreAccount()
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
            IsDeleted = true,
            CreatedAt = created.Data!.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        });

        var deleted = await new DeleteAccountUseCase(repo, uow, cache, dispatcher, factory).ExecuteAsync(id);
        Assert.True(deleted.IsSuccess);

        var rawDeleted = await ctx.Set<Account>().FirstAsync(x => x.Id == id);
        Assert.True(rawDeleted.IsDeleted);

        var eventsBefore = dispatcher.Enqueued.Count;
        var invalidatesBefore = cache.InvalidateCalls;

        var restore = new RestoreAccountUseCase(repo, uow, cache, dispatcher, factory);
        var restored = await restore.ExecuteAsync(id);

        Assert.True(restored.IsSuccess);

        var rawRestored = await ctx.Set<Account>().FirstAsync(x => x.Id == id);
        Assert.False(rawRestored.IsDeleted);
        Assert.True(rawRestored.IsActive);
        Assert.NotNull(rawRestored.UpdatedAt);

        Assert.Equal(invalidatesBefore + 1, cache.InvalidateCalls);
        Assert.False(cache.ContainsId(id));
        Assert.False(cache.ContainsCpf(request.Cpf));

        Assert.Equal(eventsBefore + 1, dispatcher.Enqueued.Count);
        Assert.Contains(dispatcher.Enqueued, e => e.GetType().Name == "AccountRestoredDomainEvent");

        var filtered = await repo.GetByIdAsync(id);
        Assert.NotNull(filtered);
    }
}