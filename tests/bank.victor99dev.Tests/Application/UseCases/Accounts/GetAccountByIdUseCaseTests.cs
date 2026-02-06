using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.CreateAccount;
using bank.victor99dev.Application.UseCases.Accounts.GetAccountById;
using bank.victor99dev.Application.UseCases.Accounts.Shared;
using bank.victor99dev.Tests.Application.Shared;
using bank.victor99dev.Tests.Infrastructure.Shared;
using Xunit;

namespace bank.victor99dev.Tests.Application.UseCases.Accounts;

public class GetAccountByIdUseCaseTests
{
    [Fact(DisplayName = "Should return account by Id and cache it when cache miss")]
    public async Task ShouldReturnAccountById_AndCacheIt_WhenCacheMiss()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var request = AccountRequests.Valid(seed: 1);
        var created = await new CreateAccountUseCase(repo, uow).ExecuteAsync(request);
        var id = created.Data!.Id;

        var cache = new FakeAccountCacheRepository();
        var useCase = new GetAccountByIdUseCase(repo, cache);

        var result = await useCase.ExecuteAsync(id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);

        Assert.Equal(id, result.Data!.Id);
        Assert.Equal("Victor Account", result.Data.Name);
        Assert.Equal(request.Cpf, result.Data.Cpf);
        Assert.True(result.Data.IsActive);
        Assert.False(result.Data.IsDeleted);

        Assert.Equal(1, cache.GetByIdCalls);
        Assert.Equal(0, cache.GetByCpfCalls);
        Assert.Equal(1, cache.SetByIdCalls);
        Assert.Equal(1, cache.SetByCpfCalls);

        Assert.True(cache.ContainsId(id));
        Assert.True(cache.ContainsCpf(request.Cpf));
    }

    [Fact(DisplayName = "Should return account from cache when cache hit")]
    public async Task ShouldReturnAccountFromCache_WhenCacheHit()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var request = AccountRequests.Valid(seed: 1);
        var created = await new CreateAccountUseCase(repo, uow).ExecuteAsync(request);
        var id = created.Data!.Id;

        var cache = new FakeAccountCacheRepository();
        cache.Seed(new AccountResponse
        {
            Id = id,
            Name = "Cached Name",
            Cpf = request.Cpf,
            IsActive = true,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        var useCase = new GetAccountByIdUseCase(repo, cache);

        var result = await useCase.ExecuteAsync(id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(id, result.Data!.Id);
        Assert.Equal("Cached Name", result.Data.Name);
        Assert.Equal(request.Cpf, result.Data.Cpf);

        Assert.Equal(1, cache.GetByIdCalls);
        Assert.Equal(0, cache.SetByIdCalls);
        Assert.Equal(0, cache.SetByCpfCalls);
    }

    [Fact(DisplayName = "Should return NotFound when account does not exist")]
    public async Task ShouldReturnNotFound_WhenAccountDoesNotExist()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, _) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var cache = new FakeAccountCacheRepository();
        var useCase = new GetAccountByIdUseCase(repo, cache);

        var result = await useCase.ExecuteAsync(Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
        Assert.Null(result.Data);

        Assert.Equal(1, cache.GetByIdCalls);
        Assert.Equal(0, cache.SetByIdCalls);
        Assert.Equal(0, cache.SetByCpfCalls);
    }
}
