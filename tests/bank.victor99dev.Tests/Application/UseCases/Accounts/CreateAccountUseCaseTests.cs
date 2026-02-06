using bank.victor99dev.Application.Shared.Messaging;
using bank.victor99dev.Application.UseCases.Accounts.CreateAccount;
using bank.victor99dev.Domain.Entities;
using bank.victor99dev.Domain.Exceptions;
using bank.victor99dev.Tests.Application.Shared;
using bank.victor99dev.Tests.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace bank.victor99dev.Tests.Application.UseCases.Accounts;

public class CreateAccountUseCaseTests
{
    [Fact(DisplayName = "Should create an account, enqueue event and invalidate cache")]
    public async Task ShouldCreateAccount_EnqueueEvent_InvalidateCache()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (ctx, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var cache = new FakeAccountCacheRepository();
        var dispatcher = new FakeDomainEventDispatcher();
        var factory = new AccountEventFactory();

        var useCase = new CreateAccountUseCase(repo, uow, cache, factory, dispatcher);
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

        Assert.Single(dispatcher.Enqueued);
        Assert.Contains(dispatcher.Enqueued, e => e.GetType().Name == "AccountCreatedDomainEvent");

        Assert.Equal(1, cache.InvalidateCalls);
    }

    [Fact(DisplayName = "Should fail when CPF is invalid")]
    public async Task ShouldFailWhenCpfIsInvalid()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var cache = new FakeAccountCacheRepository();
        var dispatcher = new FakeDomainEventDispatcher();
        var factory = new AccountEventFactory();

        var useCase = new CreateAccountUseCase(repo, uow, cache, factory, dispatcher);

        await Assert.ThrowsAsync<DomainException>(() => useCase.ExecuteAsync(AccountRequests.Valid(cpf: "123")));
    }

    [Fact(DisplayName = "Should fail when Name is invalid")]
    public async Task ShouldFailWhenNameIsInvalid()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var cache = new FakeAccountCacheRepository();
        var dispatcher = new FakeDomainEventDispatcher();
        var factory = new AccountEventFactory();

        var useCase = new CreateAccountUseCase(repo, uow, cache, factory, dispatcher);

        await Assert.ThrowsAsync<DomainException>(() => useCase.ExecuteAsync(AccountRequests.Valid(name: string.Empty)));
    }
}