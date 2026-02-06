using bank.victor99dev.Application.Interfaces.Repository;
using bank.victor99dev.Domain.Entities;
using bank.victor99dev.Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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
        AttachAsModified(account);
    }

    private void AttachAsModified(Account account)
    {
        var local = _dbContext.Set<Account>()
            .Local
            .FirstOrDefault(e => e.Id.Equals(account.Id));

        if (local is not null && !ReferenceEquals(local, account))
            _dbContext.Entry(local).State = EntityState.Detached;

        _dbContext.Attach(account);

        var entry = _dbContext.Entry(account);
        entry.State = EntityState.Modified;

        foreach (var nav in entry.Navigations)
        {
            if (!nav.Metadata.TargetEntityType.IsOwned())
                continue;

            if (nav is ReferenceEntry reference)
            {
                if (reference.CurrentValue is null)
                    continue;

                _dbContext.Entry(reference.CurrentValue).State = EntityState.Modified;
            }
            else if (nav is CollectionEntry collection)
            {
                if (collection.CurrentValue is null)
                    continue;

                foreach (var item in collection.CurrentValue)
                    _dbContext.Entry(item).State = EntityState.Modified;
            }
        }
    }
}