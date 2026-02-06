using bank.victor99dev.Application.Interfaces.CacheRepository;
using bank.victor99dev.Domain.Entities;

namespace bank.victor99dev.Application.Shared.Cache;

public static class AccountCacheInvalidation
{
    public static Task InvalidateAsync(IAccountCacheRepository cache, Account account, CancellationToken cancellationToken) => cache.InvalidateAsync(account.Id, account.Cpf.Value, cancellationToken);

    public static Task InvalidateWithOldCpfAsync(IAccountCacheRepository cache, Account account, string oldCpf, CancellationToken cancellationToken)
    {
        var tasks = new List<Task> { cache.InvalidateAsync(account.Id, account.Cpf.Value, cancellationToken) };

        if (!string.Equals(oldCpf, account.Cpf.Value, StringComparison.Ordinal))
            tasks.Add(cache.InvalidateAsync(account.Id, oldCpf, cancellationToken));

        return Task.WhenAll(tasks);
    }
}