using Essence.Base.Validation;

namespace Essence.Domain.Vocabulary;

public readonly record struct Identifier(string Key)
{
    public string Key { get; init; } = Ensure.That.IsNotNullOrEmpty(Key);
}
