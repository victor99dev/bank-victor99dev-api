using bank.victor99dev.Application.Interfaces.Caching;
using bank.victor99dev.Application.Interfaces.Messaging;
using bank.victor99dev.Application.Interfaces.Repository;
using bank.victor99dev.Application.Shared.Cache;
using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.Shared;
using bank.victor99dev.Domain.Entities;

namespace bank.victor99dev.Application.UseCases.Accounts.CreateAccount;

public class CreateAccountUseCase : ICreateAccountUseCase
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccountCacheRepository _accountCacheRepository;
    private readonly IAccountEventFactory _accountEventFactory;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    public CreateAccountUseCase(
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

    public async Task<Result<AccountResponse>> ExecuteAsync(CreateAccountRequest request, CancellationToken cancellationToken = default)
    {
        var account = Account.Create(
            accountName: request.Name,
            cpf: request.Cpf
        );

        var cpfAccount = await _accountRepository.GetByCpfAsync(account.Cpf.Value, cancellationToken);
        if (cpfAccount is not null)
            return Result.Fail<AccountResponse>($"An account with CPF {account.Cpf.Value} already exists.", ResultStatus.Conflict);

        await _accountRepository.CreateAsync(account, cancellationToken);

        await _domainEventDispatcher.EnqueueAsync([_accountEventFactory.Created(account)], cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await AccountCacheInvalidation.InvalidateAsync(_accountCacheRepository, account, cancellationToken);

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