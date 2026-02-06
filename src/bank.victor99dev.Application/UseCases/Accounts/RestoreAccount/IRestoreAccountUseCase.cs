using bank.victor99dev.Application.Shared.Results;

namespace bank.victor99dev.Application.UseCases.Accounts.RestoreAccount;

public interface IRestoreAccountUseCase
{
    Task<Result> ExecuteAsync(Guid accountId, CancellationToken cancellationToken = default);
}