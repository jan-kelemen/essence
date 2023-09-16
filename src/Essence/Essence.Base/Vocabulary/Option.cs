namespace Essence.Base.Vocabulary;

public sealed class Option<T>
{
    private readonly bool _hasValue = false;
    private readonly T? _value = default;

    public Option() { }

    public Option(T? value)
    {
        _hasValue = true;
        _value = value;
    }

    public bool HasValue => _hasValue;

    public T? Value
    {
        get
        {
            if (!HasValue)
            {
                throw new OptionException("Option does not contain a value");
            }

            return _value;
        }
    }
}
