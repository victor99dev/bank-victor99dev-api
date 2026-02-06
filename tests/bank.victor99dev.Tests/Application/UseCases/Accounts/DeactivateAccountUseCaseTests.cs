using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.CreateAccount;
using bank.victor99dev.Application.UseCases.Accounts.DeactivateAccount;
using bank.victor99dev.Domain.Exceptions;
using bank.victor99dev.Tests.Application.Shared;
using bank.victor99dev.Tests.Infrastructure.Shared;

namespace bank.victor99dev.Tests.Application.UseCases.Accounts;

public class DeactivateAccountUseCaseTests
{
    [Fact(DisplayName = "Should deactivate an active account")]
    public async Task ShouldDeactivateAccount()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();

        var (_, repoA, uowA) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var created = await new CreateAccountUseCase(repoA, uowA)
            .ExecuteAsync(AccountRequests.Valid());

        var id = created.Data!.Id;

        var (_, repoB, uowB) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var useCase = new DeactivateAccountUseCase(repoB, uowB);
        var result = await useCase.ExecuteAsync(id);

        Assert.True(result.IsSuccess);

        var (_, repoC, _) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var updated = await repoC.GetByIdAsync(id);
        Assert.NotNull(updated);
        Assert.False(updated!.IsActive);
        Assert.False(updated.IsDeleted);
        Assert.NotNull(updated.UpdatedAt);
    }

    [Fact(DisplayName = "Should return NotFound when account does not exist")]
    public async Task ShouldReturnNotFound_WhenAccountDoesNotExist()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var useCase = new DeactivateAccountUseCase(repo, uow);
        var result = await useCase.ExecuteAsync(Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact(DisplayName = "Should throw DomainException when deactivating an already inactive account")]
    public async Task ShouldThrow_WhenAlreadyInactive()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();

        var (_, repoA, uowA) = EntityFrameworkInMemoryFactory.CreateInfra(db);
        var created = await new CreateAccountUseCase(repoA, uowA)
            .ExecuteAsync(AccountRequests.Valid());

        var id = created.Data!.Id;

        var (_, repoB, uowB) = EntityFrameworkInMemoryFactory.CreateInfra(db);
        var uc1 = new DeactivateAccountUseCase(repoB, uowB);

        var first = await uc1.ExecuteAsync(id);
        Assert.True(first.IsSuccess);

        var (_, repoC, uowC) = EntityFrameworkInMemoryFactory.CreateInfra(db);
        var uc2 = new DeactivateAccountUseCase(repoC, uowC);

        await Assert.ThrowsAsync<DomainException>(() => uc2.ExecuteAsync(id));
    }

}