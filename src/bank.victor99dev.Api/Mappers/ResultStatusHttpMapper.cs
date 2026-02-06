using bank.victor99dev.Application.Shared.Results;

namespace bank.victor99dev.Api.Mappers;

public static class ResultStatusHttpMapper
{
    public static int ToHttpStatus(ResultStatus status)
    {
        switch (status)
        {
            case ResultStatus.Success:
                return StatusCodes.Status200OK;
            case ResultStatus.ValidationError:
                return StatusCodes.Status400BadRequest;
            case ResultStatus.Unauthorized:
                return StatusCodes.Status401Unauthorized;
            case ResultStatus.Forbidden:
                return StatusCodes.Status403Forbidden;
            case ResultStatus.NotFound:
                return StatusCodes.Status404NotFound;
            case ResultStatus.Conflict:
                return StatusCodes.Status409Conflict;
            case ResultStatus.UnexpectedError:
                return StatusCodes.Status500InternalServerError;
            default:
                return StatusCodes.Status500InternalServerError;
        }
    }
}
