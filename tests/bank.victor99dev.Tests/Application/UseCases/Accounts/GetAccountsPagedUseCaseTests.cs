using bank.victor99dev.Application.Shared.Messaging;
using bank.victor99dev.Application.Shared.Pagination;
using bank.victor99dev.Application.UseCases.Accounts.CreateAccount;
using bank.victor99dev.Application.UseCases.Accounts.GetAccountsPaged;
using bank.victor99dev.Tests.Application.Shared;
using bank.victor99dev.Tests.Infrastructure.Shared;

namespace bank.victor99dev.Tests.Application.UseCases.Accounts;

public class GetAccountsPagedUseCaseTests
{
    [Fact(DisplayName = "Should return paged accounts")]
    public async Task ShouldReturnPagedAccounts()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var cache = new FakeAccountCacheRepository();
        var dispatcher = new FakeDomainEventDispatcher();
        var factory = new AccountEventFactory();

        var create = new CreateAccountUseCase(repo, uow, cache, factory, dispatcher);

        for (var i = 1; i <= 5; i++)
        {
            var req = AccountRequests.Valid(name: $"Acc {i}", seed: i);
            var r = await create.ExecuteAsync(req);
            Assert.True(r.IsSuccess);
        }

        var useCase = new GetAccountsPagedUseCase(repo);
        var result = await useCase.ExecuteAsync(new PageRequest { Page = 1, PageSize = 2 });

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);

        Assert.Equal(1, result.Data!.Page);
        Assert.Equal(2, result.Data.PageSize);
        Assert.Equal(5, result.Data.TotalCount);
        Assert.Equal(2, result.Data.Items.Count);

        Assert.Contains(result.Data.Items, x => x.Name == "Acc 1");
        Assert.Contains(result.Data.Items, x => x.Name == "Acc 2");
    }

    [Fact(DisplayName = "Should return second page correctly")]
    public async Task ShouldReturnSecondPageCorrectly()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var cache = new FakeAccountCacheRepository();
        var dispatcher = new FakeDomainEventDispatcher();
        var factory = new AccountEventFactory();

        var create = new CreateAccountUseCase(repo, uow, cache, factory, dispatcher);

        for (var i = 1; i <= 5; i++)
        {
            var req = AccountRequests.Valid(name: $"Acc {i}", seed: i);
            var r = await create.ExecuteAsync(req);
            Assert.True(r.IsSuccess);
        }

        var useCase = new GetAccountsPagedUseCase(repo);
        var result = await useCase.ExecuteAsync(new PageRequest { Page = 2, PageSize = 2 });

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);

        Assert.Equal(2, result.Data!.Page);
        Assert.Equal(2, result.Data.PageSize);
        Assert.Equal(5, result.Data.TotalCount);
        Assert.Equal(2, result.Data.Items.Count);

        Assert.Contains(result.Data.Items, x => x.Name == "Acc 3");
        Assert.Contains(result.Data.Items, x => x.Name == "Acc 4");
    }
}