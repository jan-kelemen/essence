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

    public T Expect => IsSuccess ? 
        ((ValueHolder.Success)_value).Value :
        throw new ResultException("Result doesn't contain success variant");

    public E ExpectError => !IsSuccess ?
        ((ValueHolder.Error)_value).Value :
        throw new ResultException("Result doesn't contain error variant");
}
