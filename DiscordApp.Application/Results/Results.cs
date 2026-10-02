using System;
using System.Linq;
using DiscordApp.Domain.Common;

namespace DiscordApp.Application.Results;

public enum ResultType
{
    Success,
    BadRequest,
    NotFound,
    Unauthorized,
    Forbidden,
    Failure
}

public readonly record struct Result
{
    public bool IsSuccess { get; }
    public ResultType Type { get; }
    public Error[] Errors { get; }

    private Result(bool isSuccess, ResultType type, Error[] errors)
        => (IsSuccess, Type, Errors) = (isSuccess, type, errors);

    public static Result Success() => new(true, ResultType.Success, []);
    public static Result Failure(params Error[] errors) => new(false, ResultType.Failure, errors);
    public static Result NotFound(params Error[] errors) => new(false, ResultType.NotFound, errors);
    public static Result BadRequest(params Error[] errors) => new(false, ResultType.BadRequest, errors);
    public static Result Unauthorized(params Error[] errors) => new(false, ResultType.Unauthorized, errors);

    public static Result Combine(params Result[] results)
        => results.Any(r => !r.IsSuccess)
           ? Failure(results.Where(r => !r.IsSuccess).SelectMany(r => r.Errors).ToArray())
           : Success();
}

public readonly record struct Result<T>
{
    public bool IsSuccess { get; }
    public ResultType Type { get; }
    public T? Value { get; }
    public Error[] Errors { get; }

    private Result(bool isSuccess, ResultType type, T? value, Error[] errors)
        => (IsSuccess, Type, Value, Errors) = (isSuccess, type, value, errors);

    public static Result<T> Success(T value) => new(true, ResultType.Success, value, []);
    public static Result<T> Failure(Domain.Common.Error notMember, params Error[] errors) => new(false, ResultType.Failure, default, errors);
    public static Result<T> NotFound(params Error[] errors) => new(false, ResultType.NotFound, default, errors);
    public static Result<T> BadRequest(params Error[] errors) => new(false, ResultType.BadRequest, default, errors);
    public static Result<T> Unauthorized(params Error[] errors) => new(false, ResultType.Unauthorized, default, errors);

    // Functional helpers
    public Result<K> Map<K>(Func<T, K> map)
        => IsSuccess ? Result<K>.Success(map(Value!)) : new Result<K>(false, Type, default, Errors);

    public Result<K> Bind<K>(Func<T, Result<K>> next)
        => IsSuccess ? next(Value!) : new Result<K>(false, Type, default, Errors);

    public Result<T> Ensure(Func<T, bool> predicate, Error error)
        => IsSuccess && !predicate(Value!) ? BadRequest(error) : this;
}