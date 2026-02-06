using bank.victor99dev.Application.Interfaces.Repository;
using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.Shared;

namespace bank.victor99dev.Application.UseCases.Accounts.GetAccountByCpf;

public class GetAccountByCpfUseCase : IGetAccountByCpfUseCase
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountByCpfUseCase(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Result<AccountResponse>> ExecuteAsync(string cpf, CancellationToken cancellationToken = default)
    {
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

        return Result<AccountResponse>.Ok(data: response);
    }
}