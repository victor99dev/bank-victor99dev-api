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
    [Fact(DisplayName = "Should activate an inactive account and invalidate cache")]
    public async Task ShouldActivateAccount()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (ctx, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var request = AccountRequests.Valid(seed: 1);
        var created = await new CreateAccountUseCase(repo, uow).ExecuteAsync(request);
        var id = created.Data!.Id;

        var cacheDeactivate = new FakeAccountCacheRepository();

        var deactivate = new DeactivateAccountUseCase(repo, uow, cacheDeactivate);
        var deactivated = await deactivate.ExecuteAsync(id);
        Assert.True(deactivated.IsSuccess);

        var cache = new FakeAccountCacheRepository();
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

        var useCase = new ActivateAccountUseCase(repo, uow, cache);
        var result = await useCase.ExecuteAsync(id);

        Assert.True(result.IsSuccess);

        var raw = await ctx.Set<Account>().FirstAsync(x => x.Id == id);
        Assert.True(raw.IsActive);
        Assert.False(raw.IsDeleted);
        Assert.NotNull(raw.UpdatedAt);

        Assert.Equal(1, cache.InvalidateCalls);
        Assert.False(cache.ContainsId(id));
        Assert.False(cache.ContainsCpf(request.Cpf));
    }

    [Fact(DisplayName = "Should return NotFound when account does not exist")]
    public async Task ShouldReturnNotFound_WhenAccountDoesNotExist()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var cache = new FakeAccountCacheRepository();
        var useCase = new ActivateAccountUseCase(repo, uow, cache);

        var result = await useCase.ExecuteAsync(Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);

        Assert.Equal(0, cache.InvalidateCalls);
    }
}
