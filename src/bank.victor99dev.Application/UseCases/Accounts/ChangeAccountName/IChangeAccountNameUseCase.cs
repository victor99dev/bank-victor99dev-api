using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.Shared;

namespace bank.victor99dev.Application.UseCases.Accounts.ChangeAccountName;

public interface IChangeAccountNameUseCase
{
    Task<Result<AccountResponse>> ExecuteAsync(Guid accountId, ChangeAccountNameRequest request, CancellationToken cancellationToken = default);
}