
using bank.victor99dev.Application.Interfaces.Repository;
using bank.victor99dev.Infrastructure.Database.Context;
using bank.victor99dev.Infrastructure.Database.Repositories;
using bank.victor99dev.Infrastructure.Database.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace bank.victor99dev.Tests.Infrastructure.Shared;

public static class EntityFrameworkInMemoryFactory
{
    public static (AppDbContext Ctx, IAccountRepository Repo, IUnitOfWork Uow) CreateInfra(string databaseName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName)
            .EnableSensitiveDataLogging()
            .Options;

        var ctx = new AppDbContext(options);

        IAccountRepository repo = new AccountRepository(ctx);
        IUnitOfWork uow = new UnitOfWork(ctx);

        return (ctx, repo, uow);
    }

    public static string NewDbName() => $"db-{Guid.NewGuid():N}";
}
