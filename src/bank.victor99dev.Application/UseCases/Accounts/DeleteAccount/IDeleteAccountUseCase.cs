using bank.victor99dev.Application.Shared.Results;

namespace bank.victor99dev.Application.UseCases.Accounts.DeleteAccount;

public interface IDeleteAccountUseCase
{
    Task<Result> ExecuteAsync(Guid accountId, CancellationToken cancellationToken = default);
}