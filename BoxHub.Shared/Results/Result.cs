namespace BoxHub.Shared.Results;

/// <summary>
/// Result pattern đơn giản cho tầng Application / API (thành công hoặc lỗi có mã).
/// </summary>
public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? ErrorCode { get; }
    public string? ErrorMessage { get; }
    public int? HttpStatus { get; }

    private Result(bool ok, T? value, string? code, string? message, int? httpStatus)
    {
        IsSuccess = ok;
        Value = value;
        ErrorCode = code;
        ErrorMessage = message;
        HttpStatus = httpStatus;
    }

    public static Result<T> Success(T value) => new(true, value, null, null, null);

    public static Result<T> Failure(string code, string message, int httpStatus = 400) =>
        new(false, default, code, message, httpStatus);

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<string?, string?, int?, TResult> onFailure) =>
        IsSuccess ? onSuccess(Value!) : onFailure(ErrorCode, ErrorMessage, HttpStatus);
}
