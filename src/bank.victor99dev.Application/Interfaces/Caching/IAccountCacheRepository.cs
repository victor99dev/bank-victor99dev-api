using bank.victor99dev.Application.UseCases.Accounts.Shared;

namespace bank.victor99dev.Application.Interfaces.Caching;

public interface IAccountCacheRepository
{
    Task<AccountResponse?> GetByIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<AccountResponse?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default);
    Task SetAsync(AccountResponse account, TimeSpan cacheTtl, CancellationToken cancellationToken = default);
    Task InvalidateAsync(Guid accountId, string cpf, CancellationToken cancellationToken = default);
}