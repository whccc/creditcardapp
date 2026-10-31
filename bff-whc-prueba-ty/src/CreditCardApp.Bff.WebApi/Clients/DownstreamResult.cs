using Microsoft.AspNetCore.Mvc;

namespace CreditCardApp.Bff.WebApi.Clients;

public class DownstreamResult
{
    private readonly ProblemDetails? _problem;

    protected DownstreamResult(ProblemDetails? problem) => _problem = problem;

    public bool IsSuccess => _problem is null;

    public ProblemDetails Problem => _problem ?? throw new InvalidOperationException("A successful result has no problem.");

    public static DownstreamResult Success() => new(null);

    public static DownstreamResult Failure(ProblemDetails problem) => new(problem);
}

public sealed class DownstreamResult<T> : DownstreamResult
{
    private readonly T? _value;

    private DownstreamResult(T? value, ProblemDetails? problem)
        : base(problem) => _value = value;

    public T Value => IsSuccess ? _value! : throw new InvalidOperationException("A failed result has no value.");

    public static DownstreamResult<T> Success(T value) => new(value, null);

    public static new DownstreamResult<T> Failure(ProblemDetails problem) => new(default, problem);
}
