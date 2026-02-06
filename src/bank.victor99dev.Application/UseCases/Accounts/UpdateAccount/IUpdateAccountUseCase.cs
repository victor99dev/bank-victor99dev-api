using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.Shared;

namespace bank.victor99dev.Application.UseCases.Accounts.UpdateAccount;

public interface IUpdateAccountUseCase
{
    Task<Result<AccountResponse>> ExecuteAsync(Guid accountId, UpdateAccountRequest request, CancellationToken cancellationToken = default);
}