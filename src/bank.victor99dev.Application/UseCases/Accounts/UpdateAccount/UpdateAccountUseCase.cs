using bank.victor99dev.Application.Interfaces.CacheRepository;
using bank.victor99dev.Application.Interfaces.Repository;
using bank.victor99dev.Application.Shared.Cache;
using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.Shared;

namespace bank.victor99dev.Application.UseCases.Accounts.UpdateAccount;

public class UpdateAccountUseCase : IUpdateAccountUseCase
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccountCacheRepository _accountCacheRepository;
    public UpdateAccountUseCase(IAccountRepository accountRepository, IUnitOfWork unitOfWork, IAccountCacheRepository accountCacheRepository)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _accountCacheRepository = accountCacheRepository;
    }

    public async Task<Result<AccountResponse>> ExecuteAsync(Guid accountId, UpdateAccountRequest request, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account is null)
            return Result.Fail<AccountResponse>($"The account id {accountId} was not found.", ResultStatus.NotFound);
        
        var oldCpf = account.Cpf.Value;
        
        account.Update(
            accountName: request.Name,
            cpf: request.Cpf,
            isActive: request.IsActive,
            isDeleted: request.IsDeleted
        );

        _accountRepository.Update(account);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await AccountCacheInvalidation.InvalidateWithOldCpfAsync(_accountCacheRepository, account, oldCpf, cancellationToken);

        var response = new AccountResponse
        {
            Id = account.Id,
            Name = account.AccountName.Value,
            Cpf = account.Cpf.Value,
            IsActive = account.IsActive,
            IsDeleted = account.IsDeleted,
            CreatedAt = account.CreatedAt
        };

        return Result<AccountResponse>.Ok(data: response);
    }
}