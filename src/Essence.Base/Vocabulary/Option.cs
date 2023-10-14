using System.Diagnostics.CodeAnalysis;

namespace Essence.Base.Vocabulary;

public sealed class Option<T> where T : notnull
{
    private record ValueHolder
    {
        public record Some(T Value) : ValueHolder;

        public record None() : ValueHolder;
    }

    private readonly ValueHolder _value;

    public Option()
    {
        _value = new ValueHolder.None();
    }

    public Option(T value)
    {
        _value = new ValueHolder.Some(value);
    }

    public static implicit operator Option<T>(T value) => new(value);

    public bool HasValue() => _value is ValueHolder.Some;

    public bool HasValue([NotNullWhen(returnValue: true)] out T? value)
    {
        if (_value is ValueHolder.Some some)
        {
            value = some.Value;
            return true;
        }
        value = default;
        return false;
    }

    public T Value()
    {
        if (HasValue(out T? value))
        {
            return value;
        }
        throw new OptionException("Option does not contain a value");
    }
}
