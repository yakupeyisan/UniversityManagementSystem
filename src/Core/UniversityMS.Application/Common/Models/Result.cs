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

    public static Result<T> Success<T>(T data, string message = "İşlem başarılı.")
        => new(true, data, message, Array.Empty<string>());

    public static Result<T> Failure<T>(string message, params string[] errors)
        => new(false, default!, message, errors);
}

public class Result<T> : Result
{
    public T Data { get; }

    protected internal Result(bool isSuccess, T data, string message, string[] errors)
        : base(isSuccess, message, errors)
    {
        Data = data;
    }
}