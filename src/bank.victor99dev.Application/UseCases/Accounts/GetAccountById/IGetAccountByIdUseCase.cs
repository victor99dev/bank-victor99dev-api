using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.Shared;

namespace bank.victor99dev.Application.UseCases.Accounts.GetAccountById;

public interface IGetAccountByIdUseCase
{
    Task<Result<AccountResponse>> ExecuteAsync(Guid accountId, CancellationToken cancellationToken = default);
}