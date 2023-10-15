using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Essence.Base.Validation;
using Essence.Domain.Vocabulary;

namespace Essence.Domain.Model;
public readonly record struct RecipeHeader(Identifier Id, string Name)
{
    public string Name { get; } = Ensure.That.IsNotNullOrWhitespace(Name).Trim();
}

public readonly record struct RecipeIngredient(IngredientHeader Ingredient, uint Amount)
{
    public uint Amount { get; } = Ensure.That.InRange<uint>(Amount, 0, int.MaxValue);
}

public class Recipe
{
    private readonly RecipeHeader _header;
    private readonly string _description = string.Empty;
    private readonly ImmutableArray<RecipeIngredient> _ingredients = ImmutableArray<RecipeIngredient>.Empty;

    public Recipe(RecipeHeader header)
    {
        _header = header;
    }

    public Identifier Id => _header.Id;

    public string Name => _header.Name;

    public string Description
    {
        get => _description;
        init => _description = Ensure.That.IsNotNull(value);
    }

    public IEnumerable<RecipeIngredient> Ingredients
    {
        get => _ingredients;
        init => _ingredients = CombineIngredients(Ensure.That.IsNotNull(value));
    }

    private static ImmutableArray<RecipeIngredient> CombineIngredients(IEnumerable<RecipeIngredient> ingredients)
    {
        return ingredients.GroupBy(i => i.Ingredient,
            (IngredientHeader id, IEnumerable<RecipeIngredient> combiled) =>
            {
                return new RecipeIngredient(id, (uint)combiled.Sum(a => a.Amount));
            }).ToImmutableArray();
    }
}
