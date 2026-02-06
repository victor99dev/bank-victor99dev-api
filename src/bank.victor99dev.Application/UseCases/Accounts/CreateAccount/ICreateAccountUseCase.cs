using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.Shared;

namespace bank.victor99dev.Application.UseCases.Accounts.CreateAccount;

public interface ICreateAccountUseCase
{
    Task<Result<AccountResponse>> ExecuteAsync(CreateAccountRequest request, CancellationToken cancellationToken = default);
}
