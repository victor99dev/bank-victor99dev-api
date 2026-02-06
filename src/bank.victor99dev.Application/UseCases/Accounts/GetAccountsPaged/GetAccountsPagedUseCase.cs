using bank.victor99dev.Application.Shared.Pagination;
using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.Shared;

namespace bank.victor99dev.Application.UseCases.Accounts.GetAccountsPaged;

public class GetAccountsPagedUseCase : IGetAccountsPagedUseCase
{
    public Task<Result<PageResult<AccountResponse>>> ExecuteAsync(PageRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}