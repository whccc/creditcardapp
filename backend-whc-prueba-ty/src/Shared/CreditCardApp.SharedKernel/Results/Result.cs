namespace CreditCardApp.SharedKernel.Results;

public class Result
{
    private readonly Error? _error;

    protected Result(bool isSuccess, Error? error)
    {
        if (isSuccess == error is not null)
        {
            throw new ArgumentException("A successful result cannot carry an error and a failed result must carry one.", nameof(error));
        }

        IsSuccess = isSuccess;
        _error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error => _error ?? throw new InvalidOperationException("A successful result has no error.");

    public static Result Success() => new(true, null);

    public static Result Failure(Error error) => new(false, error);

    public static implicit operator Result(Error error) => Failure(error);
}
