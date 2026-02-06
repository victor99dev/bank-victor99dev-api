using bank.victor99dev.Application.UseCases.Accounts.CreateAccount;
using bank.victor99dev.Application.UseCases.Accounts.GetAccountById;
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
    public AccountsController(
        ILogger<AccountsController> logger,
        ICreateAccountUseCase createAccountUseCase,
        IGetAccountByIdUseCase getAccountByIdUseCase)
    {
        _logger = logger;
        _createAccountUseCase = createAccountUseCase;
        _getAccountByIdUseCase = getAccountByIdUseCase;
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
}