using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.Shared;

namespace bank.victor99dev.Application.UseCases.Accounts.GetAccountByCpf;

public interface IGetAccountByCpfUseCase
{
    Task<Result<AccountResponse>> ExecuteAsync(string cpf, CancellationToken cancellationToken = default);
}