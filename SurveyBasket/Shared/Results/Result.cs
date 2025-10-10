
namespace SurveyBasket.Shared.Results;

public abstract record Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    protected Result(bool isSuccess, Error error)
    {
        if ((isSuccess && error != Error.None) || (!isSuccess && error == Error.None))
            throw new InvalidOperationException("Invalid Result state.");
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new SuccessResult();
    public static Result Failure(Error error) => new FailureResult(error);
    public static Result<T> Success<T>(T value) => new SuccessResult<T>(value);
    public static Result<T> Failure<T>(Error error) => new FailureResult<T>(error);
}
public abstract record Result<T> : Result
{
    protected Result(bool isSuccess, Error error) : base(isSuccess, error) { }
}

