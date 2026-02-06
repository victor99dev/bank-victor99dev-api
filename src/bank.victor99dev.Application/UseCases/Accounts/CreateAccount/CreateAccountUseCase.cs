using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.Shared;

namespace bank.victor99dev.Application.UseCases.Accounts.CreateAccount;

public class CreateAccountUseCase : ICreateAccountUseCase
{
    public Task<Result<AccountResponse>> ExecuteAsync(CreateAccountRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}