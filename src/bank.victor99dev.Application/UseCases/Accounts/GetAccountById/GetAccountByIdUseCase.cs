using bank.victor99dev.Application.Interfaces.CacheRepository;
using bank.victor99dev.Application.Interfaces.Repository;
using bank.victor99dev.Application.Shared.Cache;
using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.Shared;

namespace bank.victor99dev.Application.UseCases.Accounts.GetAccountById;

public class GetAccountByIdUseCase : IGetAccountByIdUseCase
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAccountCacheRepository _accountCacheRepository;
    public GetAccountByIdUseCase(IAccountRepository accountRepository, IAccountCacheRepository accountCacheRepository)
    {
        _accountRepository = accountRepository;
        _accountCacheRepository = accountCacheRepository;
    }

    public async Task<Result<AccountResponse>> ExecuteAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var cached = await _accountCacheRepository.GetByIdAsync(accountId, cancellationToken);
        if (cached is not null)
            return Result<AccountResponse>.Ok(data: cached);
        
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account is null)
            return Result.Fail<AccountResponse>($"The account id {accountId} was not found.", ResultStatus.NotFound);

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

        var ttl = CacheTtl.UntilEndOfDayUtc();
        await _accountCacheRepository.SetByIdAsync(response, ttl, cancellationToken);
        await _accountCacheRepository.SetByCpfAsync(response.Cpf, response, ttl, cancellationToken);

        return Result<AccountResponse>.Ok(data: response);
    }
}