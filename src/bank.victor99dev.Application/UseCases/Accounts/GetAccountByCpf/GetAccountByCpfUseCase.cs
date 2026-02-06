using bank.victor99dev.Application.Interfaces.CacheRepository;
using bank.victor99dev.Application.Interfaces.Repository;
using bank.victor99dev.Application.Shared.Cache;
using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.Shared;

namespace bank.victor99dev.Application.UseCases.Accounts.GetAccountByCpf;

public class GetAccountByCpfUseCase : IGetAccountByCpfUseCase
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAccountCacheRepository _accountCacheRepository;
    public GetAccountByCpfUseCase(IAccountRepository accountRepository, IAccountCacheRepository accountCacheRepository)
    {
        _accountRepository = accountRepository;
        _accountCacheRepository = accountCacheRepository;
    }

    public async Task<Result<AccountResponse>> ExecuteAsync(string cpf, CancellationToken cancellationToken = default)
    {
        var cached = await _accountCacheRepository.GetByCpfAsync(cpf, cancellationToken);
        if (cached is not null)
            return Result<AccountResponse>.Ok(data: cached);

        var account = await _accountRepository.GetByCpfAsync(cpf, cancellationToken);
        if (account is null)
            return Result.Fail<AccountResponse>($"The account with CPF {cpf} was not found.", ResultStatus.NotFound);

        var response = new AccountResponse
        {
            Id = account.Id,
            Name = account.AccountName.Value,
            Cpf = account.Cpf.Value,
            IsActive = account.IsActive,
            IsDeleted = account.IsDeleted,
            CreatedAt = account.CreatedAt,
            UpdatedAt = account.UpdatedAt
        };

        await _accountCacheRepository.SetAsync(response, CacheTtl.CacheExpiresAtEndOfDayUtc(), cancellationToken);

        return Result<AccountResponse>.Ok(data: response);
    }
}