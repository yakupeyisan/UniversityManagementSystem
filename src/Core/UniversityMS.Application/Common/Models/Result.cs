namespace UniversityMS.Application.Common.Models;
public class Result
{
    public bool IsSuccess { get; }
    public string Message { get; }
    public string[] Errors { get; }

    protected Result(bool isSuccess, string message, string[] errors)
    {
        IsSuccess = isSuccess;
        Message = message;
        Errors = errors;
    }

    public static Result Success(string message = "İşlem başarılı.")
        => new(true, message, Array.Empty<string>());

    public static Result Failure(string message, params string[] errors)
        => new(false, message, errors);

}

/// <summary>
/// Genel sonuç modeli - başarı/başarısızlık
/// </summary>
public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string? Message { get; private set; }
    public List<string> Errors { get; private set; } = new();

    private Result() { }

    public static Result<T> Success(T data, string? message = null)
    {
        return new Result<T>
        {
            IsSuccess = true,
            Data = data,
            Message = message ?? "İşlem başarılı"
        };
    }

    public static Result<T> Failure(string message)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Message = message,
            Errors = new List<string> { message }
        };
    }

    public static Result<T> Failure(List<string> errors)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Message = "Birden fazla hata oluştu",
            Errors = errors
        };
    }
    public static Result<T> Failure(string message,List<string> errors)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Message = message,
            Errors = errors
        };
    }
}
