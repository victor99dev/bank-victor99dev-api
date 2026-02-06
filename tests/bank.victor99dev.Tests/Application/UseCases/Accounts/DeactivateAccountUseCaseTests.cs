using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.CreateAccount;
using bank.victor99dev.Application.UseCases.Accounts.DeactivateAccount;
using bank.victor99dev.Domain.Exceptions;
using bank.victor99dev.Tests.Application.Shared;
using bank.victor99dev.Tests.Infrastructure.Shared;

namespace bank.victor99dev.Tests.Application.UseCases.Accounts;

public class DeactivateAccountUseCaseTests
{
    [Fact(DisplayName = "Should deactivate an active account and invalidate cache")]
    public async Task ShouldDeactivateAccount()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();

        var (_, repoA, uowA) = EntityFrameworkInMemoryFactory.CreateInfra(db);
        var request = AccountRequests.Valid(seed: 1);

        var created = await new CreateAccountUseCase(repoA, uowA).ExecuteAsync(request);
        var id = created.Data!.Id;

        var cache = new FakeAccountCacheRepository();
        
        cache.Seed(new()
        {
            Id = id,
            Name = created.Data!.Name,
            Cpf = request.Cpf,
            IsActive = true,
            IsDeleted = false,
            CreatedAt = created.Data!.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        });

        var (_, repoB, uowB) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var useCase = new DeactivateAccountUseCase(repoB, uowB, cache);
        var result = await useCase.ExecuteAsync(id);

        Assert.True(result.IsSuccess);

        Assert.Equal(1, cache.InvalidateCalls);
        Assert.False(cache.ContainsId(id));
        Assert.False(cache.ContainsCpf(request.Cpf));

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

        var cache = new FakeAccountCacheRepository();
        var useCase = new DeactivateAccountUseCase(repo, uow, cache);

        var result = await useCase.ExecuteAsync(Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);

        Assert.Equal(0, cache.InvalidateCalls);
    }

    [Fact(DisplayName = "Should throw DomainException when deactivating an already inactive account")]
    public async Task ShouldThrow_WhenAlreadyInactive()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();

        var (_, repoA, uowA) = EntityFrameworkInMemoryFactory.CreateInfra(db);
        var request = AccountRequests.Valid(seed: 1);

        var created = await new CreateAccountUseCase(repoA, uowA).ExecuteAsync(request);
        var id = created.Data!.Id;

        var cache = new FakeAccountCacheRepository();

        var (_, repoB, uowB) = EntityFrameworkInMemoryFactory.CreateInfra(db);
        var uc1 = new DeactivateAccountUseCase(repoB, uowB, cache);

        var first = await uc1.ExecuteAsync(id);
        Assert.True(first.IsSuccess);

        var (_, repoC, uowC) = EntityFrameworkInMemoryFactory.CreateInfra(db);
        var uc2 = new DeactivateAccountUseCase(repoC, uowC, cache);

        await Assert.ThrowsAsync<DomainException>(() => uc2.ExecuteAsync(id));
    }
}
