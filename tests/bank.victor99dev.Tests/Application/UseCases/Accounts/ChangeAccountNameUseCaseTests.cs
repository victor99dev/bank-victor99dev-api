using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.ChangeAccountName;
using bank.victor99dev.Application.UseCases.Accounts.CreateAccount;
using bank.victor99dev.Tests.Application.Shared;
using bank.victor99dev.Tests.Infrastructure.Shared;

namespace bank.victor99dev.Tests.Application.UseCases.Accounts;

public class ChangeAccountNameUseCaseTests
{
    [Fact(DisplayName = "Should change account name and invalidate cache for id and cpf")]
    public async Task ShouldChangeName_AndInvalidateCache()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var request = AccountRequests.Valid(seed: 1);
        var created = await new CreateAccountUseCase(repo, uow).ExecuteAsync(request);

        var id = created.Data!.Id;
        var cpf = request.Cpf;

        var cache = new FakeAccountCacheRepository();

        cache.Seed(new()
        {
            Id = id,
            Name = created.Data!.Name,
            Cpf = cpf,
            IsActive = true,
            IsDeleted = false,
            CreatedAt = created.Data!.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        });

        var useCase = new ChangeAccountNameUseCase(repo, uow, cache);

        var change = new ChangeAccountNameRequest
        {
            Name = "New Name"
        };

        var result = await useCase.ExecuteAsync(id, change);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);

        Assert.Equal(id, result.Data!.Id);
        Assert.Equal("New Name", result.Data.Name);
        Assert.Equal(cpf, result.Data.Cpf);

        Assert.Equal(2, cache.InvalidateCalls);
        Assert.False(cache.ContainsId(id));
        Assert.False(cache.ContainsCpf(cpf));
    }

    [Fact(DisplayName = "Should return NotFound when account does not exist")]
    public async Task ShouldReturnNotFound_WhenAccountDoesNotExist()
    {
        var db = EntityFrameworkInMemoryFactory.NewDbName();
        var (_, repo, uow) = EntityFrameworkInMemoryFactory.CreateInfra(db);

        var cache = new FakeAccountCacheRepository();
        var useCase = new ChangeAccountNameUseCase(repo, uow, cache);

        var change = new ChangeAccountNameRequest
        {
            Name = "New Name"
        };

        var result = await useCase.ExecuteAsync(Guid.NewGuid(), change);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
        Assert.Null(result.Data);

        Assert.Equal(0, cache.InvalidateCalls);
    }
}
