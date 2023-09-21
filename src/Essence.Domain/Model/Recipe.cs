using Essence.Base.Validation;
using Essence.Domain.Vocabulary;

namespace Essence.Domain.Model;

public class Recipe
{
    private string _description = string.Empty;

    public Recipe(Identifier id, string name)
    {
        Id = Ensure.That.IsNotNull(id);
        Name = Ensure.That.IsNotNullOrWhitespace(name).Trim();
    }

    public Identifier Id { get; }

    public string Name { get; }

    public string Description
    {
        get => _description;
        init => _description = Ensure.That.IsNotNull(value);
    }
}
