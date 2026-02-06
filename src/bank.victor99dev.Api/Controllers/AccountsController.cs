using bank.victor99dev.Application.UseCases.Accounts.CreateAccount;
using bank.victor99dev.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace bank.victor99dev.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : BaseApiController
{
    private readonly ILogger<AccountsController> _logger;
    private readonly ICreateAccountUseCase _createAccountUseCase;
    public AccountsController(
        ILogger<AccountsController> logger, 
        ICreateAccountUseCase createAccountUseCase)
    {
        _logger = logger;
        _createAccountUseCase = createAccountUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCondominium([FromBody] CreateAccountRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _createAccountUseCase.ExecuteAsync(request, cancellationToken);
            
            LogResult(_logger, result, nameof(CreateCondominium));
            
            return FromResult(result);
        }
        catch (DomainException ex)
        {
            LogDomain(_logger, ex, nameof(CreateCondominium));
            return DomainProblem(ex);
        }
        catch (Exception ex)
        {
            LogUnexpected(_logger, ex, nameof(CreateCondominium));
            return UnexpectedProblem();
        }
    }
}