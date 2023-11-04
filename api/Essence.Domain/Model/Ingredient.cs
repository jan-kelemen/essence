using Essence.Base.Validation;
using Essence.Domain.Vocabulary;

namespace Essence.Domain.Model;

public readonly record struct IngredientHeader(Identifier Id, string Name)
{
    public string Name { get; } = Ensure.That.IsNotNullOrWhitespace(Name).Trim();
}

public class Ingredient
{
    private readonly IngredientHeader _header;
    private readonly string? _summary = null;
    private readonly string? _description = null;

    public Ingredient(IngredientHeader header, string? summary = null, string? description = null)
    {
        _header = header;
        _summary = summary;
        _description = description;
    }

    public Identifier Id => _header.Id;

    public string Name => _header.Name;

    public string? Summary => _summary;

    public string? Description => _description;
}
