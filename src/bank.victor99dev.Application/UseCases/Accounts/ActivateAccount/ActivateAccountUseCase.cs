using bank.victor99dev.Application.Shared.Results;

namespace bank.victor99dev.Application.UseCases.Accounts.ActivateAccount;

public class ActivateAccountUseCase : IActivateAccountUseCase
{
    public Task<Result> ExecuteAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}