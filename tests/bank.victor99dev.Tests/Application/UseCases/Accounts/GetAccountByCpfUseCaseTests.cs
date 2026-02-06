using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.CreateAccount;
using bank.victor99dev.Application.UseCases.Accounts.GetAccountByCpf;
using bank.victor99dev.Tests.Application.Shared;
using bank.victor99dev.Tests.Infrastructure.Shared;

namespace bank.victor99dev.Tests.Application.UseCases.Accounts;

public class GetAccountByCpfUseCaseTests
{
    [Fact(DisplayName = "Should return account by CPF")]
    public async Task ShouldReturnAccountByCpf()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var request = AccountRequests.Valid(seed: 1);
        var created = await new CreateAccountUseCase(repo, uow).ExecuteAsync(request);
        var id = created.Data!.Id;

        var useCase = new GetAccountByCpfUseCase(repo);
        var result = await useCase.ExecuteAsync(request.Cpf);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);

        Assert.Equal(id, result.Data!.Id);
        Assert.Equal(request.Name, result.Data.Name);
        Assert.Equal(request.Cpf, result.Data.Cpf);
    }

    [Fact(DisplayName = "Should return NotFound when CPF does not exist")]
    public async Task ShouldReturnNotFound_WhenCpfDoesNotExist()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, _) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var useCase = new GetAccountByCpfUseCase(repo);
        var result = await useCase.ExecuteAsync("39053344705");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
        Assert.Null(result.Data);
    }
}