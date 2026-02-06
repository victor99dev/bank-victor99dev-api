using bank.victor99dev.Application.Interfaces.Caching;
using bank.victor99dev.Application.Interfaces.Messaging;
using bank.victor99dev.Application.Interfaces.Repository;
using bank.victor99dev.Application.Shared.Cache;
using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.Shared;

namespace bank.victor99dev.Application.UseCases.Accounts.ChangeAccountName;

public class ChangeAccountNameUseCase : IChangeAccountNameUseCase
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccountCacheRepository _accountCacheRepository;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly IAccountEventFactory _accountEventFactory;
    public ChangeAccountNameUseCase(
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork,
        IAccountCacheRepository accountCacheRepository,
        IDomainEventDispatcher domainEventDispatcher,
        IAccountEventFactory accountEventFactory)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _accountCacheRepository = accountCacheRepository;
        _domainEventDispatcher = domainEventDispatcher;
        _accountEventFactory = accountEventFactory;
    }

    public async Task<Result<AccountResponse>> ExecuteAsync(Guid accountId, ChangeAccountNameRequest request, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account is null)
            return Result.Fail<AccountResponse>($"The account id {accountId} was not found.", ResultStatus.NotFound);

        var oldCpf = account.Cpf.Value;
        var oldName = account.AccountName.Value;

        account.ChangeName(request.Name);

        _accountRepository.Update(account);

        await _domainEventDispatcher.EnqueueAsync([
                _accountEventFactory.NameChanged(account, oldName),
                _accountEventFactory.Updated(account)],
            cancellationToken: cancellationToken
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await AccountCacheInvalidation.InvalidateWithOldCpfAsync(_accountCacheRepository, account, oldCpf, cancellationToken);

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

        return Result<AccountResponse>.Ok(data: response);
    }
}