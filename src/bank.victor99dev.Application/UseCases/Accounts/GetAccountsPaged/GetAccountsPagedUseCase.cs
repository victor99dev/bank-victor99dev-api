using bank.victor99dev.Application.Interfaces.Repository;
using bank.victor99dev.Application.Shared.Pagination;
using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Application.UseCases.Accounts.Shared;

namespace bank.victor99dev.Application.UseCases.Accounts.GetAccountsPaged;

public class GetAccountsPagedUseCase : IGetAccountsPagedUseCase
{
    private readonly IAccountRepository _accountRepository;
    public GetAccountsPagedUseCase(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Result<PageResult<AccountResponse>>> ExecuteAsync(PageRequest request, CancellationToken cancellationToken = default)
    {
        var (Items, TotalCount) = await _accountRepository.GetPagedAsync(request.Page, request.PageSize, cancellationToken);

        var response = new PageResult<AccountResponse>{
        Items = Items
                .Select(a => new AccountResponse
                {
                    Id = a.Id,
                    Name = a.AccountName.Value,
                    Cpf = a.Cpf.Value,
                    IsActive = a.IsActive,
                    IsDeleted = a.IsDeleted,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt
                })
                .ToList(),

            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = TotalCount
        };

        return Result<PageResult<AccountResponse>>.Ok(data: response);
    }
}