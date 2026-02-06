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
    [Fact(DisplayName = "Should activate an inactive account")]
    public async Task ShouldActivateAccount()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (ctx, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var created = await new CreateAccountUseCase(repo, uow).ExecuteAsync(AccountRequests.Valid());
        var id = created.Data!.Id;

        var deactivate = new DeactivateAccountUseCase(repo, uow);
        var deactivated = await deactivate.ExecuteAsync(id);
        Assert.True(deactivated.IsSuccess);

        var useCase = new ActivateAccountUseCase(repo, uow);
        var result = await useCase.ExecuteAsync(id);

        Assert.True(result.IsSuccess);

        var raw = await ctx.Set<Account>().FirstAsync(x => x.Id == id);
        Assert.True(raw.IsActive);
        Assert.False(raw.IsDeleted);
        Assert.NotNull(raw.UpdatedAt);
    }

    [Fact(DisplayName = "Should return NotFound when account does not exist")]
    public async Task ShouldReturnNotFound_WhenAccountDoesNotExist()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var useCase = new ActivateAccountUseCase(repo, uow);
        var result = await useCase.ExecuteAsync(Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }
}