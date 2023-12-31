﻿using Essence.Base.Validation;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Essence.Base.Vocabulary;

public readonly struct SuccessDiscriminator { };
public readonly struct ErrorDiscriminator { };

public sealed class Result<T, E>
{
    private record ValueHolder
    {
        public record Success(T Value) : ValueHolder;
        public record Error(E Value) : ValueHolder;
    }

    private readonly ValueHolder _value;

    public Result(T success) : this(new SuccessDiscriminator(), success) { }

    public Result(E error) : this(new ErrorDiscriminator(), error) { }

    public Result(SuccessDiscriminator _, T value)
    {
        _value = new ValueHolder.Success(value);
    }

    public Result(ErrorDiscriminator _, E value)
    {
        _value = new ValueHolder.Error(value);
    }

    public static implicit operator Result<T, E>(T success) => new(success);

    public static implicit operator Result<T, E>(E error) => new(error);

    public bool IsSuccess => _value is ValueHolder.Success;

    public bool IsError => !IsSuccess;

    public T Expect()
    {
        var success = _value as ValueHolder.Success;
        if (success is not null)
        {
            return success.Value;
        }
        throw new ResultException("Result doesn't contain success variant");
    }

    public bool Expect([NotNullWhen(returnValue: true)] out T? value)
    {
        if (_value is ValueHolder.Success success)
        {
            value = success.Value!;
            return true;
        }
        value = default;
        return false;
    }

    public E ExpectError()
    {
        var err = _value as ValueHolder.Error;
        if (err is not null)
        {
            return err.Value;
        }
        throw new ResultException("Result doesn't contain error variant");
    }

    public bool ExpectError([NotNullWhen(returnValue: true)] out E? value)
    {
        if (_value is ValueHolder.Error err)
        {
            value = err.Value!;
            return true;
        }
        value = default;
        return false;
    }

    public Result<U, E> Map<U>(Func<T, U> map)
    {
        Ensure.That.IsNotNull(map);

        if (Expect(out T? value))
        {
            return new Result<U, E>(new SuccessDiscriminator(), map(value));
        }
        return new Result<U, E>(new ErrorDiscriminator(), ExpectErrorUnchecked());
    }

    public Result<T, F> MapError<F>(Func<E, F> mapError)
    {
        Ensure.That.IsNotNull(mapError);

        if (ExpectError(out E? value))
        {
            return new Result<T, F>(new ErrorDiscriminator(), mapError(value));
        }
        return new Result<T, F>(new SuccessDiscriminator(), ExpectUnchecked());
    }

    public U MapOr<U>(Func<T, U> map, U @default)
    {
        Ensure.That.IsNotNull(map);

        if (Expect(out T? value))
        {
            return map(value);
        }
        return @default;
    }

    public U MapOrElse<U>(Func<T, U> mapSuccess, Func<E, U> mapError)
    {
        Ensure.That.IsNotNull(mapSuccess);
        Ensure.That.IsNotNull(mapError);

        if (Expect(out T? value))
        {
            return mapSuccess(value);
        }
        return mapError(ExpectError());
    }

    private T ExpectUnchecked() => ((ValueHolder.Success)_value).Value;

    private E ExpectErrorUnchecked() => ((ValueHolder.Error)_value).Value;
}
