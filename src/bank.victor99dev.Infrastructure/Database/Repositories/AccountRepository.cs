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

    public async Task<Account?> GetByCpfAsync(string cpf, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<Account>()
         .AsNoTracking()
         .FirstOrDefaultAsync(e => e.Cpf.Value.Equals(cpf), cancellationToken);
    }

    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Account>()
         .AsNoTracking()
         .FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
    }

    public async Task<(IReadOnlyList<Account> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Accounts.AsNoTracking();

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
    public void Update(Account account)
    {
        account.SetUpdatedAt();
        AttachAsModified(account);
    }

    private void AttachAsModified(Account account)
    {
        var local = _dbContext.Set<Account>()
            .Local
            .FirstOrDefault(e => e.Id == account.Id);

        if (local is not null && !ReferenceEquals(local, account))
            _dbContext.Entry(local).State = EntityState.Detached;

        _dbContext.Attach(account);
        _dbContext.Entry(account).State = EntityState.Modified;
    }
}