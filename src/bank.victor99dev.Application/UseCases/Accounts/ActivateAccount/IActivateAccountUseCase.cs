using bank.victor99dev.Application.Shared.Results;

namespace bank.victor99dev.Application.UseCases.Accounts.ActivateAccount;

public interface IActivateAccountUseCase
{
    Task<Result> ExecuteAsync(Guid accountId, CancellationToken cancellationToken = default);
}