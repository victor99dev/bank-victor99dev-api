using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.Shared;

namespace bank.victor99dev.Application.UseCases.Accounts.ChangeAccountCpf;

public interface IChangeAccountCpfUseCase
{
    Task<Result<AccountResponse>> ExecuteAsync(Guid accountId, ChangeAccountCpfRequest request, CancellationToken cancellationToken = default);
}