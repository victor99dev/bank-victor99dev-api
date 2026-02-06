using System.Text.Json;
using bank.victor99dev.Application.Interfaces.CacheRepository;
using bank.victor99dev.Application.UseCases.Accounts.Shared;
using Microsoft.Extensions.Caching.Distributed;

namespace bank.victor99dev.Infrastructure.DatabaseCache.Repositories;

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
        var json = await _cache.GetStringAsync(Keys.ById(accountId), cancellationToken);
        return string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize<AccountResponse>(json, JsonOptions);
    }

    public async Task<AccountResponse?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default)
    {
        var normalized = cpf.Trim();
        var json = await _cache.GetStringAsync(Keys.ByCpf(normalized), cancellationToken);
        return string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize<AccountResponse>(json, JsonOptions);
    }


    public async Task InvalidateAsync(Guid accountId, string cpf, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(Keys.ById(accountId), cancellationToken);
        await _cache.RemoveAsync(Keys.ByCpf(cpf.Trim()), cancellationToken);
    }

    public async Task SetAsync(AccountResponse account, TimeSpan cacheTtl, CancellationToken cancellationToken = default)
    {
        var opts = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = cacheTtl };
        var json = JsonSerializer.Serialize(account, JsonOptions);

        await _cache.SetStringAsync(Keys.ById(account.Id), json, opts, cancellationToken);
        await _cache.SetStringAsync(Keys.ByCpf(account.Cpf.Trim()), json, opts, cancellationToken);
    }

    private static class Keys
    {
        public static string ById(Guid id) => $"accounts:id:{id}";
        public static string ByCpf(string cpf) => $"accounts:cpf:{cpf}";
    }
}