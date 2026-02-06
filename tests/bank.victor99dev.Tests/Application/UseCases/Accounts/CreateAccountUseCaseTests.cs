using bank.victor99dev.Application.UseCases.Accounts.CreateAccount;
using bank.victor99dev.Domain.Entities;
using bank.victor99dev.Domain.Exceptions;
using bank.victor99dev.Tests.Application.Shared;
using bank.victor99dev.Tests.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace bank.victor99dev.Tests.Application.UseCases.Accounts;

public class CreateAccountUseCaseTests
{
    [Fact(DisplayName = "Should create an account")]
    public async Task ShouldCreateAccount()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (ctx, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var useCase = new CreateAccountUseCase(repo, uow);

        var request = AccountRequests.Valid(seed: 1);
        var result = await useCase.ExecuteAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.NotEqual(Guid.Empty, result.Data!.Id);

        var raw = await ctx.Set<Account>().FirstAsync(x => x.Id == result.Data!.Id);

        Assert.Equal("Victor Account", raw.AccountName.Value);
        Assert.Equal(request.Cpf, raw.Cpf.Value);
        Assert.True(raw.IsActive);
        Assert.False(raw.IsDeleted);
        Assert.NotEqual(default, raw.CreatedAt);
    }

    [Fact(DisplayName = "Should fail when CPF is invalid")]
    public async Task ShouldFailWhenCpfIsInvalid()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var useCase = new CreateAccountUseCase(repo, uow);

        await Assert.ThrowsAsync<DomainException>(() =>
            useCase.ExecuteAsync(AccountRequests.Valid(cpf: "123")));
    }

    [Fact(DisplayName = "Should fail when Name is invalid")]
    public async Task ShouldFailWhenNameIsInvalid()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var useCase = new CreateAccountUseCase(repo, uow);

        await Assert.ThrowsAsync<DomainException>(() => useCase.ExecuteAsync(AccountRequests.Valid(name: string.Empty)));
    }

     [Fact(DisplayName = "Should fail when CPF is invalid (all same digits)")]
    public async Task ShouldFail_WhenCpfIsInvalid_AllSameDigits()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var useCase = new CreateAccountUseCase(repo, uow);

        await Assert.ThrowsAsync<DomainException>(() =>
            useCase.ExecuteAsync(AccountRequests.Valid(cpf: "11111111111")));
    }

    [Fact(DisplayName = "Should fail when CPF has less than 11 digits")]
    public async Task ShouldFail_WhenCpfHasLessThan11Digits()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var useCase = new CreateAccountUseCase(repo, uow);

        await Assert.ThrowsAsync<DomainException>(() =>
            useCase.ExecuteAsync(AccountRequests.Valid(cpf: "123456789")));
    }

    [Fact(DisplayName = "Should fail when CPF is null or empty")]
    public async Task ShouldFail_WhenCpfIsNullOrEmpty()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var useCase = new CreateAccountUseCase(repo, uow);

        await Assert.ThrowsAsync<DomainException>(() =>
            useCase.ExecuteAsync(AccountRequests.Valid(cpf: "")));
    }
}
