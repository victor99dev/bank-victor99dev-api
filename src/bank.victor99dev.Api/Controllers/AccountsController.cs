using bank.victor99dev.Application.Shared.Pagination;
using bank.victor99dev.Application.UseCases.Accounts.ActivateAccount;
using bank.victor99dev.Application.UseCases.Accounts.CreateAccount;
using bank.victor99dev.Application.UseCases.Accounts.DeactivateAccount;
using bank.victor99dev.Application.UseCases.Accounts.DeleteAccount;
using bank.victor99dev.Application.UseCases.Accounts.GetAccountByCpf;
using bank.victor99dev.Application.UseCases.Accounts.GetAccountById;
using bank.victor99dev.Application.UseCases.Accounts.GetAccountsPaged;
using bank.victor99dev.Application.UseCases.Accounts.RestoreAccount;
using bank.victor99dev.Application.UseCases.Accounts.UpdateAccount;
using bank.victor99dev.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace bank.victor99dev.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : BaseApiController
{
    private readonly ILogger<AccountsController> _logger;
    private readonly ICreateAccountUseCase _createAccountUseCase;
    private readonly IGetAccountByIdUseCase _getAccountByIdUseCase;
    private readonly IGetAccountsPagedUseCase _getAccountsPagedUseCase;
    private readonly IDeleteAccountUseCase _deleteAccountUseCase;
    private readonly IRestoreAccountUseCase _restoreAccountUseCase;
    private readonly IDeactivateAccountUseCase _deactivateAccountUseCase;
    private readonly IActivateAccountUseCase _activateAccountUseCase;
    private readonly IGetAccountByCpfUseCase _getAccountByCpfUseCase;
    private readonly IUpdateAccountUseCase _updateAccountUseCase;
    public AccountsController(
        ILogger<AccountsController> logger,
        ICreateAccountUseCase createAccountUseCase,
        IGetAccountByIdUseCase getAccountByIdUseCase,
        IGetAccountsPagedUseCase getAccountsPagedUseCase,
        IDeleteAccountUseCase deleteAccountUseCase,
        IRestoreAccountUseCase restoreAccountUseCase,
        IDeactivateAccountUseCase deactivateAccountUseCase,
        IActivateAccountUseCase activateAccountUseCase,
        IGetAccountByCpfUseCase getAccountByCpfUseCase,
        IUpdateAccountUseCase updateAccountUseCase)
    {
        _logger = logger;
        _createAccountUseCase = createAccountUseCase;
        _getAccountByIdUseCase = getAccountByIdUseCase;
        _getAccountsPagedUseCase = getAccountsPagedUseCase;
        _deleteAccountUseCase = deleteAccountUseCase;
        _restoreAccountUseCase = restoreAccountUseCase;
        _deactivateAccountUseCase = deactivateAccountUseCase;
        _activateAccountUseCase = activateAccountUseCase;
        _getAccountByCpfUseCase = getAccountByCpfUseCase;
        _updateAccountUseCase = updateAccountUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _createAccountUseCase.ExecuteAsync(request, cancellationToken);

            LogResult(_logger, result, nameof(CreateAccount));

            return FromResult(result);
        }
        catch (DomainException ex)
        {
            LogDomain(_logger, ex, nameof(CreateAccount));
            return DomainProblem(ex);
        }
        catch (Exception ex)
        {
            LogUnexpected(_logger, ex, nameof(CreateAccount));
            return UnexpectedProblem();
        }
    }


    [HttpGet("{accountId}")]
    public async Task<IActionResult> GetAccountById([FromRoute] Guid accountId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _getAccountByIdUseCase.ExecuteAsync(accountId, cancellationToken);

            LogResult(_logger, result, nameof(GetAccountById));
            return FromResult(result);
        }
        catch (DomainException ex)
        {
            LogDomain(_logger, ex, nameof(GetAccountById));
            return DomainProblem(ex);
        }
        catch (Exception ex)
        {
            LogUnexpected(_logger, ex, nameof(GetAccountById));
            return UnexpectedProblem();
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAccountPaged([FromQuery] PageRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _getAccountsPagedUseCase.ExecuteAsync(request, cancellationToken);

            LogResult(_logger, result, nameof(GetAccountPaged));
            return FromResult(result);
        }
        catch (DomainException ex)
        {
            LogDomain(_logger, ex, nameof(GetAccountPaged));
            return DomainProblem(ex);
        }
        catch (Exception ex)
        {
            LogUnexpected(_logger, ex, nameof(GetAccountPaged));
            return UnexpectedProblem();
        }
    }

    [HttpDelete("{accountId}")]
    public async Task<IActionResult> DeleteAccount([FromRoute] Guid accountId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _deleteAccountUseCase.ExecuteAsync(accountId, cancellationToken);

            LogResult(_logger, result, nameof(DeleteAccount));
            return FromResult(result);
        }
        catch (DomainException ex)
        {
            LogDomain(_logger, ex, nameof(DeleteAccount));
            return DomainProblem(ex);
        }
        catch (Exception ex)
        {
            LogUnexpected(_logger, ex, nameof(DeleteAccount));
            return UnexpectedProblem();
        }
    }

    [HttpPatch("{accountId}/restore")]
    public async Task<IActionResult> RestoreAccount([FromRoute] Guid accountId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _restoreAccountUseCase.ExecuteAsync(accountId, cancellationToken);

            LogResult(_logger, result, nameof(RestoreAccount));
            return FromResult(result);
        }
        catch (DomainException ex)
        {
            LogDomain(_logger, ex, nameof(RestoreAccount));
            return DomainProblem(ex);
        }
        catch (Exception ex)
        {
            LogUnexpected(_logger, ex, nameof(RestoreAccount));
            return UnexpectedProblem();
        }
    }

    [HttpPatch("{accountId}/deactivate")]
    public async Task<IActionResult> DeactivateAccount([FromRoute] Guid accountId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _deactivateAccountUseCase.ExecuteAsync(accountId, cancellationToken);

            LogResult(_logger, result, nameof(DeactivateAccount));
            return FromResult(result);
        }
        catch (DomainException ex)
        {
            LogDomain(_logger, ex, nameof(DeactivateAccount));
            return DomainProblem(ex);
        }
        catch (Exception ex)
        {
            LogUnexpected(_logger, ex, nameof(DeactivateAccount));
            return UnexpectedProblem();
        }
    }

    [HttpPatch("{accountId}/activate")]
    public async Task<IActionResult> ActivateAccount([FromRoute] Guid accountId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _activateAccountUseCase.ExecuteAsync(accountId, cancellationToken);

            LogResult(_logger, result, nameof(ActivateAccount));
            return FromResult(result);
        }
        catch (DomainException ex)
        {
            LogDomain(_logger, ex, nameof(ActivateAccount));
            return DomainProblem(ex);
        }
        catch (Exception ex)
        {
            LogUnexpected(_logger, ex, nameof(ActivateAccount));
            return UnexpectedProblem();
        }
    }

    [HttpGet("{cpf}/cpf")]
    public async Task<IActionResult> GetByCpf([FromRoute] string cpf, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _getAccountByCpfUseCase.ExecuteAsync(cpf, cancellationToken);

            LogResult(_logger, result, nameof(GetByCpf));
            return FromResult(result);
        }
        catch (DomainException ex)
        {
            LogDomain(_logger, ex, nameof(GetByCpf));
            return DomainProblem(ex);
        }
        catch (Exception ex)
        {
            LogUnexpected(_logger, ex, nameof(GetByCpf));
            return UnexpectedProblem();
        }
    }

    [HttpPut("{accountId}")]
    public async Task<IActionResult> UpdateAccount([FromRoute] Guid accountId, [FromBody] UpdateAccountRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var requestBody = new UpdateAccountRequest
            {
                Name = request.Name,
                Cpf = request.Cpf,
                IsActive = request.IsActive,
                IsDeleted = request.IsDeleted
            };

            var result = await _updateAccountUseCase.ExecuteAsync(accountId, requestBody, cancellationToken);

            LogResult(_logger, result, nameof(UpdateAccount));
            return FromResult(result);
        }
        catch (DomainException ex)
        {
            LogDomain(_logger, ex, nameof(UpdateAccount));
            return DomainProblem(ex);
        }
        catch (Exception ex)
        {
            LogUnexpected(_logger, ex, nameof(UpdateAccount));
            return UnexpectedProblem();
        }
    }
}