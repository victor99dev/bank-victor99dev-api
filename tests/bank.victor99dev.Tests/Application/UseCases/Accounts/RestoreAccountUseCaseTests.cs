using bank.victor99dev.Application.UseCases.Accounts.CreateAccount;
using bank.victor99dev.Application.UseCases.Accounts.DeleteAccount;
using bank.victor99dev.Application.UseCases.Accounts.RestoreAccount;
using bank.victor99dev.Application.UseCases.Accounts.Shared;
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

        var request = AccountRequests.Valid(seed: 1);
        var created = await new CreateAccountUseCase(repo, uow).ExecuteAsync(request);
        var id = created.Data!.Id;

        var cache = new FakeAccountCacheRepository();

        cache.Seed(new AccountResponse
        {
            Id = id,
            Name = created.Data!.Name,
            Cpf = request.Cpf,
            IsActive = true,
            IsDeleted = true,
            CreatedAt = created.Data!.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        });

        var deleted = await new DeleteAccountUseCase(repo, uow).ExecuteAsync(id);
        Assert.True(deleted.IsSuccess);

        var rawDeleted = await ctx.Set<Account>().FirstAsync(x => x.Id == id);
        Assert.True(rawDeleted.IsDeleted);

        var restore = new RestoreAccountUseCase(repo, uow, cache);
        var restored = await restore.ExecuteAsync(id);

        Assert.True(restored.IsSuccess);

        var rawRestored = await ctx.Set<Account>().FirstAsync(x => x.Id == id);
        Assert.False(rawRestored.IsDeleted);
        Assert.True(rawRestored.IsActive);
        Assert.NotNull(rawRestored.UpdatedAt);

        Assert.Equal(1, cache.InvalidateCalls);
        Assert.False(cache.ContainsId(id));
        Assert.False(cache.ContainsCpf(request.Cpf));

        var filtered = await repo.GetByIdAsync(id);
        Assert.NotNull(filtered);
    }
}
