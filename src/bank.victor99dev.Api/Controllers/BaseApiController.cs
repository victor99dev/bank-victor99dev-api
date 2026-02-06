using bank.victor99dev.Api.Mappers;
using bank.victor99dev.Application.Shared.Results;
using bank.victor99dev.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace bank.victor99dev.Api.Controllers;

[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class BaseApiController : ControllerBase
{
    protected const string UnexpectedErrorDetail = "An unexpected error occurred while processing the request.";

    protected const string ErrorDetail = "Invalid data for the requested operation.";

    protected IActionResult FromResult(Result result)
    {
        if (result.IsSuccess)
            return NoContent();

        return Problem(
            title: result.Title,
            detail: result.Mensagem,
            statusCode: ResultStatusHttpMapper.ToHttpStatus(result.Status)
        );
    }

    protected IActionResult FromResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(result.Data);

        return Problem(
            title: result.Title,
            detail: result.Mensagem,
            statusCode: ResultStatusHttpMapper.ToHttpStatus(result.Status)
        );
    }

    protected IActionResult DomainProblem(DomainException ex, int statusCode = StatusCodes.Status422UnprocessableEntity)
        => Problem(title: ErrorDetail, detail: ex.Message, statusCode: statusCode);

    protected IActionResult UnexpectedProblem()
        => Problem(title: "Internal Server Error", detail: UnexpectedErrorDetail, statusCode: StatusCodes.Status500InternalServerError);

    protected void LogDomain(ILogger logger, Exception ex, string operation)
        => logger.LogWarning(ex,
            "Domain error in {Operation} | Message={Message} | traceId={TraceId} | route={Route}",
            operation,
            ex.Message,
            HttpContext.TraceIdentifier,
            HttpContext.Request?.Path.Value);

    protected void LogUnexpected(ILogger logger, Exception ex, string operation)
        => logger.LogError(ex,
            "Unexpected error in {Operation} | Message={Message} | traceId={TraceId} | route={Route}",
            operation,
            ex.Message,
            HttpContext.TraceIdentifier,
            HttpContext.Request?.Path.Value);

    protected void LogResult(ILogger logger, Result result, string operation)
    {
        if (result.IsSuccess)
            return;

        logger.LogWarning(
            "Operation failed | Operation={Operation} | Status={Status} | Title={Title} | Message={Message} | traceId={TraceId} | route={Route}",
            operation,
            result.Status,
            result.Title,
            result.Mensagem,
            HttpContext.TraceIdentifier,
            HttpContext.Request?.Path.Value
        );
    }

    protected void LogResult<T>(ILogger logger, Result<T> result, string operation)
    {
        if (result.IsSuccess)
            return;

        logger.LogWarning(
            "Operation failed | Operation={Operation} | Status={Status} | Title={Title} | Message={Message} | traceId={TraceId} | route={Route}",
            operation,
            result.Status,
            result.Title,
            result.Mensagem,
            HttpContext.TraceIdentifier,
            HttpContext.Request?.Path.Value
        );
    }
}
