using bank.victor99dev.Application.Interfaces.Repository;
using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.Shared;
using bank.victor99dev.Domain.Entities;

namespace bank.victor99dev.Application.UseCases.Accounts.CreateAccount;

public class CreateAccountUseCase : ICreateAccountUseCase
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    public CreateAccountUseCase(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AccountResponse>> ExecuteAsync(CreateAccountRequest request, CancellationToken cancellationToken = default)
    {
        var account  = Account.Create(
            accountName: request.Name,
            cpf: request.Cpf
        );

        await _accountRepository.CreateAsync(account, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new AccountResponse{
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