namespace SurveyBasket.Shared.Results;

public sealed record SuccessResult() : Result(true, Error.None);
public sealed record SuccessResult<T> : Result<T>
{
    public T Value { get; }

    public SuccessResult(T value) : base(true, Error.None)
    {
        Value = value;
    }
}
