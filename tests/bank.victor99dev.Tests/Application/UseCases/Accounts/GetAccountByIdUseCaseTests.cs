using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.CreateAccount;
using bank.victor99dev.Application.UseCases.Accounts.GetAccountById;
using bank.victor99dev.Tests.Application.Shared;
using bank.victor99dev.Tests.Infrastructure.Shared;

namespace bank.victor99dev.Tests.Application.UseCases.Accounts;

public class GetAccountByIdUseCaseTests
{
    [Fact(DisplayName = "Should return account by Id")]
    public async Task ShouldReturnAccountById()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var request = AccountRequests.Valid(seed: 1);
        var created = await new CreateAccountUseCase(repo, uow).ExecuteAsync(request);
        var id = created.Data!.Id;

        var useCase = new GetAccountByIdUseCase(repo);
        var result = await useCase.ExecuteAsync(id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);

        Assert.Equal(id, result.Data!.Id);
        Assert.Equal("Victor Account", result.Data.Name);
        Assert.Equal(request.Cpf, result.Data.Cpf);
        Assert.True(result.Data.IsActive);
        Assert.False(result.Data.IsDeleted);
    }

    [Fact(DisplayName = "Should return NotFound when account does not exist")]
    public async Task ShouldReturnNotFound_WhenAccountDoesNotExist()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, _) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var useCase = new GetAccountByIdUseCase(repo);
        var result = await useCase.ExecuteAsync(Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
        Assert.Null(result.Data);
    }
}