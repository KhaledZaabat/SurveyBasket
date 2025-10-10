

public sealed record FailureResult(Error Error) : Result(false, Error);


public sealed record FailureResult<T>(Error Error) : Result<T>(false, Error);