using bank.victor99dev.Application.Interfaces.Repository;
using bank.victor99dev.Domain.Entities;
using bank.victor99dev.Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace bank.victor99dev.Infrastructure.Database.Repositories;

public class AccountRepository : IAccountRepository
{
    protected readonly AppDbContext _dbContext;
    public AccountRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Account> CreateAsync(Account entity, CancellationToken cancellationToken = default)
    {
            await _dbContext.AddAsync(entity, cancellationToken);
            return entity;
    }

    public async Task<IEnumerable<Account>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Account>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
           return await _dbContext.Set<Account>()
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
    }

    public void Remove(Account account) => _dbContext.Accounts.Update(account);
    
    public void Update(Account account) => _dbContext.Accounts.Remove(account);
}