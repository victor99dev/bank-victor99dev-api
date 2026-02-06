using bank.victor99dev.Application.Interfaces.Caching;
using bank.victor99dev.Application.UseCases.Accounts.Shared;

namespace bank.victor99dev.Tests.Infrastructure.Shared;

public sealed class FakeAccountCacheRepository : IAccountCacheRepository
{
    private readonly Dictionary<Guid, AccountResponse> _byId = new();
    private readonly Dictionary<string, AccountResponse> _byCpf = new(StringComparer.Ordinal);

    public int GetByIdCalls { get; private set; }
    public int GetByCpfCalls { get; private set; }
    public int SetCalls { get; private set; }
    public int InvalidateCalls { get; private set; }

    public Task<AccountResponse?> GetByIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        GetByIdCalls++;
        _byId.TryGetValue(accountId, out var value);
        return Task.FromResult(value);
    }

    public Task<AccountResponse?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default)
    {
        GetByCpfCalls++;
        var normalized = cpf.Trim();
        _byCpf.TryGetValue(normalized, out var value);
        return Task.FromResult(value);
    }

    public Task SetAsync(AccountResponse account, TimeSpan cacheTtl, CancellationToken cancellationToken = default)
    {
        SetCalls++;
        _byId[account.Id] = account;
        _byCpf[account.Cpf.Trim()] = account;
        return Task.CompletedTask;
    }

    public Task InvalidateAsync(Guid accountId, string cpf, CancellationToken cancellationToken = default)
    {
        InvalidateCalls++;
        _byId.Remove(accountId);
        _byCpf.Remove(cpf.Trim());
        return Task.CompletedTask;
    }

    public void Seed(AccountResponse account)
    {
        _byId[account.Id] = account;
        _byCpf[account.Cpf.Trim()] = account;
    }

    public bool ContainsId(Guid id) => _byId.ContainsKey(id);
    public bool ContainsCpf(string cpf) => _byCpf.ContainsKey(cpf.Trim());
}
