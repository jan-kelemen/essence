namespace Essence.Base.Vocabulary;

public sealed class Option<T>
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

    public bool HasValue => _value is ValueHolder.Some;

    public T Value => HasValue ?
        ((ValueHolder.Some)_value).Value :
        throw new OptionException("Option does not contain a value");
}
