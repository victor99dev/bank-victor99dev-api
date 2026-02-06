namespace bank.victor99dev.Application.Shared.Results;

public enum ResultStatus
{
    Success,
    ValidationError,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden,
    UnexpectedError
}