using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.CreateAccount;
using bank.victor99dev.Application.UseCases.Accounts.UpdateAccount;
using bank.victor99dev.Tests.Application.Shared;
using bank.victor99dev.Tests.Infrastructure.Shared;

namespace bank.victor99dev.Tests.Application.UseCases.Accounts;

public class UpdateAccountUseCaseTests
{
    [Fact(DisplayName = "Should update account and invalidate cache for id, new cpf and old cpf when cpf changes")]
    public async Task ShouldUpdateAccount_AndInvalidateCache_WithOldCpf()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var request = AccountRequests.Valid(seed: 1);
        var created = await new CreateAccountUseCase(repo, uow).ExecuteAsync(request);

        var id = created.Data!.Id;
        var oldCpf = request.Cpf;

        var cache = new FakeAccountCacheRepository();

        cache.Seed(new()
        {
            Id = id,
            Name = created.Data!.Name,
            Cpf = oldCpf,
            IsActive = true,
            IsDeleted = false,
            CreatedAt = created.Data!.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        });

        var useCase = new UpdateAccountUseCase(repo, uow, cache);

        var update = new UpdateAccountRequest
        {
            Name = "New Name",
            Cpf = "39053344705",
            IsActive = true,
            IsDeleted = false
        };

        var result = await useCase.ExecuteAsync(id, update);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);

        Assert.Equal(id, result.Data!.Id);
        Assert.Equal("New Name", result.Data.Name);
        Assert.Equal("39053344705", result.Data.Cpf);

        Assert.Equal(2, cache.InvalidateCalls);
        Assert.False(cache.ContainsId(id));
        Assert.False(cache.ContainsCpf(oldCpf));
        Assert.False(cache.ContainsCpf("39053344705"));
    }

    [Fact(DisplayName = "Should return NotFound when account does not exist")]
    public async Task ShouldReturnNotFound_WhenAccountDoesNotExist()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var cache = new FakeAccountCacheRepository();
        var useCase = new UpdateAccountUseCase(repo, uow, cache);

        var update = new UpdateAccountRequest
        {
            Name = "New Name",
            Cpf = "39053344705",
            IsActive = true,
            IsDeleted = false
        };

        var result = await useCase.ExecuteAsync(Guid.NewGuid(), update);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
        Assert.Null(result.Data);

        Assert.Equal(0, cache.InvalidateCalls);
    }
}
