using bank.victor99dev.Application.UseCases.Accounts.Shared;

namespace bank.victor99dev.Application.Interfaces.CacheRepository;

public interface IAccountCacheRepository
{
    Task<AccountResponse?> GetByIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<AccountResponse?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default);
    Task SetByIdAsync(AccountResponse account, TimeSpan ttl, CancellationToken cancellationToken = default);
    Task SetByCpfAsync(string cpf, AccountResponse account, TimeSpan ttl, CancellationToken cancellationToken = default);
    Task InvalidateAsync(Guid accountId, string cpf, CancellationToken cancellationToken = default);
}