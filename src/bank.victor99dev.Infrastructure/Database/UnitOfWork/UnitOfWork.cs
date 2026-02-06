using bank.victor99dev.Application.Interfaces.Repository;
using bank.victor99dev.Infrastructure.Database.Context;

namespace bank.victor99dev.Infrastructure.Database.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    protected readonly AppDbContext _dbContext;
    public UnitOfWork(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => await _dbContext.SaveChangesAsync(cancellationToken);
}