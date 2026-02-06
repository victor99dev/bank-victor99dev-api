using bank.victor99dev.Application.Shared.Messaging;
using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.CreateAccount;
using bank.victor99dev.Application.UseCases.Accounts.GetAccountByCpf;
using bank.victor99dev.Application.UseCases.Accounts.Shared;
using bank.victor99dev.Tests.Application.Shared;
using bank.victor99dev.Tests.Infrastructure.Shared;

namespace bank.victor99dev.Tests.Application.UseCases.Accounts;

public class GetAccountByCpfUseCaseTests
{
    [Fact(DisplayName = "Should return account by CPF and cache it when cache miss")]
    public async Task ShouldReturnAccountByCpf_AndCacheIt_WhenCacheMiss()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var cache = new FakeAccountCacheRepository();
        var dispatcher = new FakeDomainEventDispatcher();
        var factory = new AccountEventFactory();

        var request = AccountRequests.Valid(seed: 1);
        var created = await new CreateAccountUseCase(repo, uow, cache, factory, dispatcher).ExecuteAsync(request);
        var id = created.Data!.Id;

        var useCase = new GetAccountByCpfUseCase(repo, cache);

        var result = await useCase.ExecuteAsync(request.Cpf);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);

        Assert.Equal(id, result.Data!.Id);
        Assert.Equal(request.Name, result.Data.Name);
        Assert.Equal(request.Cpf, result.Data.Cpf);

        Assert.Equal(1, cache.GetByCpfCalls);
        Assert.Equal(0, cache.GetByIdCalls);
        Assert.Equal(1, cache.SetCalls);

        Assert.True(cache.ContainsCpf(request.Cpf));
        Assert.True(cache.ContainsId(id));
    }

    [Fact(DisplayName = "Should return account from cache when cache hit")]
    public async Task ShouldReturnAccountFromCache_WhenCacheHit()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var cache = new FakeAccountCacheRepository();
        var dispatcher = new FakeDomainEventDispatcher();
        var factory = new AccountEventFactory();

        var request = AccountRequests.Valid(seed: 1);
        var created = await new CreateAccountUseCase(repo, uow, cache, factory, dispatcher).ExecuteAsync(request);
        var id = created.Data!.Id;

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

        var setBefore = cache.SetCalls;

        var useCase = new GetAccountByCpfUseCase(repo, cache);
        var result = await useCase.ExecuteAsync(request.Cpf);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);

        Assert.Equal(id, result.Data!.Id);
        Assert.Equal("Cached Name", result.Data.Name);
        Assert.Equal(request.Cpf, result.Data.Cpf);

        Assert.Equal(1, cache.GetByCpfCalls);
        Assert.Equal(setBefore, cache.SetCalls);
    }

    [Fact(DisplayName = "Should return NotFound when CPF does not exist")]
    public async Task ShouldReturnNotFound_WhenCpfDoesNotExist()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, _) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var cache = new FakeAccountCacheRepository();
        var useCase = new GetAccountByCpfUseCase(repo, cache);

        var result = await useCase.ExecuteAsync("39053344705");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
        Assert.Null(result.Data);

        Assert.Equal(1, cache.GetByCpfCalls);
        Assert.Equal(0, cache.SetCalls);
    }
}
