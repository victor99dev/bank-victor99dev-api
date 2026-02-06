using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.Shared;

namespace bank.victor99dev.Application.UseCases.Accounts.GetAccountById;

public class GetAccountByIdUseCase : IGetAccountByIdUseCase
{
    public Task<Result<AccountResponse>> ExecuteAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}