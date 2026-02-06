namespace bank.victor99dev.Application.Shared.Results;

public sealed class Result
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public string? Mensagem { get; private set; }
    public ResultStatus Status { get; private set; }
    public string? Title { get; private set; }
    public string? TraceId { get; private set; }

    public static Result Ok(string? mensagem = null) =>
        new()
        {
            IsSuccess = true,
            Status = ResultStatus.Success,
            Mensagem = mensagem
        };

    public static Result Fail(
        string mensagem,
        ResultStatus status,
        string? title = null,
        string? traceId = null) =>
        new()
        {
            IsSuccess = false,
            Mensagem = mensagem,
            Status = status,
            Title = title,
            TraceId = traceId
        };

    public static Result<T> Ok<T>(T data, string? mensagem = null) =>
        Result<T>.Ok(data, mensagem);

    public static Result<T> Fail<T>(
        string mensagem,
        ResultStatus status,
        string? title = null,
        string? traceId = null) =>
        Result<T>.Fail(mensagem, status, title, traceId);
}

public sealed class Result<T>
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public string? Mensagem { get; private set; }
    public ResultStatus Status { get; private set; }
    public T? Data { get; private set; }
    public string? Title { get; private set; }
    public string? TraceId { get; private set; }

    public static Result<T> Ok(string? mensagem = null) =>
        new()
        {
            IsSuccess = true,
            Status = ResultStatus.Success,
            Data = default,
            Mensagem = mensagem
        };

    public static Result<T> Ok(T data, string? mensagem = null) =>
        new()
        {
            IsSuccess = true,
            Status = ResultStatus.Success,
            Data = data,
            Mensagem = mensagem
        };

    public static Result<T> Fail(
        string mensagem,
        ResultStatus status,
        string? title = null,
        string? traceId = null) =>
        new()
        {
            IsSuccess = false,
            Status = status,
            Mensagem = mensagem,
            Title = title,
            TraceId = traceId
        };
}
