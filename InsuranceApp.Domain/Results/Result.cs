

namespace InsuranceApp.Domain.Results;

public readonly struct Result<T>
{
    private readonly T? _value;

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access Value when the result is a failure.");

    public IReadOnlyList<Error> Errors { get; }

    private Result(bool isSuccess, T? value, IReadOnlyList<Error> errors)
    {
        IsSuccess = isSuccess;
        _value = value;
        Errors = errors;
    }

    public static Result<T> Ok(T value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value), "Success value cannot be null. Use OkNullable if you intend to allow null.");

        return new Result<T>(true, value, []);
    }

    public static Result<T> OkNullable(T? value) =>
        new(true, value, []);

    public static Result<T> Fail(Error error) =>
        new(false, default, [error]);

    public static Result<T> Fail(params Error[] errors) =>
        new(false, default, NormalizeErrors(errors));

    public static Result<T> Fail(IReadOnlyList<Error> errors) =>
        new(false, default, NormalizeErrors(errors));

    public Result<TResult> Map<TResult>(Func<T, TResult> mapper)
    {
        ArgumentNullException.ThrowIfNull(mapper);
        return IsSuccess ? Result<TResult>.OkNullable(mapper(_value!)) : Result<TResult>.Fail(Errors);
    }

    public Result<TResult> Bind<TResult>(Func<T, Result<TResult>> binder)
    {
        ArgumentNullException.ThrowIfNull(binder);
        return IsSuccess ? binder(_value!) : Result<TResult>.Fail(Errors);
    }

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<IReadOnlyList<Error>, TResult> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);
        return IsSuccess ? onSuccess(_value!) : onFailure(Errors);
    }

    public Result<T> Tap(Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        if (IsSuccess) action(_value!);
        return this;
    }

    public Result<T> TapFailure(Action<IReadOnlyList<Error>> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        if (IsFailure) action(Errors);
        return this;
    }

    public bool TryGetValue(out T value)
    {
        value = _value!;
        return IsSuccess;
    }

    public override string ToString()
    {
        if (IsSuccess)
        {
            var valueText = _value is null ? "null" : _value.ToString();
            return $"Ok({valueText})";
        }

        return $"Fail({string.Join(", ", Errors)})";
    }
        

    public static implicit operator Result<T>(T value) => Ok(value);

    private static Error[] NormalizeErrors(Error[]? errors) =>
        errors is not null && errors.Length != 0 ? errors : [Error.Unspecified()];

    private static IReadOnlyList<Error> NormalizeErrors(IReadOnlyList<Error>? errors) =>
        errors is not null && errors.Count != 0 ? errors : [Error.Unspecified()];
}

public readonly struct Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public IReadOnlyList<Error> Errors { get; }

    private Result(bool isSuccess, IReadOnlyList<Error> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }

    public static Result Ok() => new(true, []);

    public static Result Fail(Error error) => new(false, [error]);

    public static Result Fail(params Error[] errors) => new(false, NormalizeErrors(errors));

    public static Result Fail(IReadOnlyList<Error> errors) => new(false, NormalizeErrors(errors));

    public Result<T> ToResult<T>(T value) => IsSuccess ? Result<T>.Ok(value) : Result<T>.Fail(Errors);

    public TResult Match<TResult>(Func<TResult> onSuccess, Func<IReadOnlyList<Error>, TResult> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);
        return IsSuccess ? onSuccess() : onFailure(Errors);
    }

    public override string ToString() =>
        IsSuccess ? "Ok" : $"Fail({string.Join(", ", Errors)})";

    private static Error[] NormalizeErrors(Error[]? errors) =>
        errors is not null && errors.Length != 0 ? errors : [Error.Unspecified()];

    private static IReadOnlyList<Error> NormalizeErrors(IReadOnlyList<Error>? errors) =>
        errors is not null && errors.Count != 0 ? errors : [Error.Unspecified()];
}



