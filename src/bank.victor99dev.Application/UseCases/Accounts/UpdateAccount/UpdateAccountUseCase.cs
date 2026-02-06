using bank.victor99dev.Application.Interfaces.Caching;
using bank.victor99dev.Application.Interfaces.Messaging;
using bank.victor99dev.Application.Interfaces.Repository;
using bank.victor99dev.Application.Shared.Cache;
using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.Shared;
using bank.victor99dev.Domain.Primitives;

namespace bank.victor99dev.Application.UseCases.Accounts.UpdateAccount;

public class UpdateAccountUseCase : IUpdateAccountUseCase
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccountCacheRepository _accountCacheRepository;
    private readonly IAccountEventFactory _accountEventFactory;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    public UpdateAccountUseCase(
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork,
        IAccountCacheRepository accountCacheRepository,
        IAccountEventFactory accountEventFactory,
        IDomainEventDispatcher domainEventDispatcher)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _accountCacheRepository = accountCacheRepository;
        _accountEventFactory = accountEventFactory;
        _domainEventDispatcher = domainEventDispatcher;
    }

    public async Task<Result<AccountResponse>> ExecuteAsync(Guid accountId, UpdateAccountRequest request, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account is null)
            return Result.Fail<AccountResponse>($"The account id {accountId} was not found.", ResultStatus.NotFound);

        var oldCpf = account.Cpf.Value;
        var oldName = account.AccountName.Value;

        account.Update(
            accountName: request.Name,
            cpf: request.Cpf,
            isActive: request.IsActive,
            isDeleted: request.IsDeleted
        );

        _accountRepository.Update(account);

        var events = new List<IDomainEvent>();

        if (!string.Equals(oldCpf, account.Cpf.Value, StringComparison.Ordinal))
            events.Add(_accountEventFactory.CpfChanged(account, oldCpf));

        if (!string.Equals(oldName, account.AccountName.Value, StringComparison.Ordinal))
            events.Add(_accountEventFactory.NameChanged(account, oldName));

        events.Add(_accountEventFactory.Updated(account));

        await AccountCacheInvalidation.InvalidateWithOldCpfAsync(_accountCacheRepository, account, oldCpf, cancellationToken);
        await _domainEventDispatcher.EnqueueAsync(events, cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

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