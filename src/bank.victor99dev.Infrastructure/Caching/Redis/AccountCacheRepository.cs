using System.Text.Json;
using bank.victor99dev.Application.Interfaces.Caching;
using bank.victor99dev.Application.UseCases.Accounts.Shared;
using Microsoft.Extensions.Caching.Distributed;

namespace bank.victor99dev.Infrastructure.Caching.Redis;

public class AccountCacheRepository : IAccountCacheRepository
{
    private readonly IDistributedCache _cache;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    public AccountCacheRepository(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<AccountResponse?> GetByIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var json = await _cache.GetStringAsync(RedisKeyBuilder.AccountById(accountId), cancellationToken);
        return string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize<AccountResponse>(json, JsonOptions);
    }

    public async Task<AccountResponse?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default)
    {
        var normalized = cpf.Trim();
        var json = await _cache.GetStringAsync(RedisKeyBuilder.AccountByCpf(normalized), cancellationToken);
        return string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize<AccountResponse>(json, JsonOptions);
    }


    public async Task InvalidateAsync(Guid accountId, string cpf, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(RedisKeyBuilder.AccountById(accountId), cancellationToken);
        await _cache.RemoveAsync(RedisKeyBuilder.AccountByCpf(cpf.Trim()), cancellationToken);
    }

    public async Task SetAsync(AccountResponse account, TimeSpan cacheTtl, CancellationToken cancellationToken = default)
    {
        var opts = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = cacheTtl };
        var json = JsonSerializer.Serialize(account, JsonOptions);

        await _cache.SetStringAsync(RedisKeyBuilder.AccountById(account.Id), json, opts, cancellationToken);
        await _cache.SetStringAsync(RedisKeyBuilder.AccountByCpf(account.Cpf.Trim()), json, opts, cancellationToken);
    }
}