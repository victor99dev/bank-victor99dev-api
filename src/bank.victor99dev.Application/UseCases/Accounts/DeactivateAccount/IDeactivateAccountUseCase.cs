using bank.victor99dev.Application.Shared.Results;

namespace bank.victor99dev.Application.UseCases.Accounts.DeactivateAccount;

public interface IDeactivateAccountUseCase
{
    Task<Result> ExecuteAsync(Guid accountId, CancellationToken cancellationToken = default);
}