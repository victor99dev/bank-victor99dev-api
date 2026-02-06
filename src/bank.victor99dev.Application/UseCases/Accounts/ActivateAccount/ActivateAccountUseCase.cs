using bank.victor99dev.Application.Interfaces.CacheRepository;
using bank.victor99dev.Application.Interfaces.Repository;
using bank.victor99dev.Application.Shared.Cache;
using bank.victor99dev.Application.Shared.Results;

namespace bank.victor99dev.Application.UseCases.Accounts.ActivateAccount;

public class ActivateAccountUseCase : IActivateAccountUseCase
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccountCacheRepository _accountCacheRepository;
    public ActivateAccountUseCase(IAccountRepository accountRepository, IUnitOfWork unitOfWork, IAccountCacheRepository accountCacheRepository)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _accountCacheRepository = accountCacheRepository;
    }

    public async Task<Result> ExecuteAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account is null)
            return Result.Fail($"The account id {accountId} was not found.", ResultStatus.NotFound);

        account.Activate();

        _accountRepository.Update(account);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await AccountCacheInvalidation.InvalidateAsync(_accountCacheRepository, account, cancellationToken);

        return Result.Ok();
    }
}