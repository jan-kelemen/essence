using Essence.Base.Validation;
using Essence.Base.Vocabulary;
using Essence.Domain.Model;
using Essence.Domain.Vocabulary;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Essence.Domain.Services;

public readonly record struct AddRecipe(string Name, string Description, IEnumerable<RecipeIngredient> Ingredients)
{
    public string Name { get; init; } = Ensure.That.IsNotNullOrWhitespace(Name);

    public string Description { get; init; } = Ensure.That.IsNotNull(Description);

    public IEnumerable<RecipeIngredient> Ingredients { get; init; } = Ensure.That.IsNotNull(Ingredients);
}

public record AddRecipeError
{
    public record Conflict(RecipeHeader? ConflictingRecipe) : AddRecipeError;

    public record UnknownIngredients(IEnumerable<RecipeIngredient> Ingredients) : AddRecipeError;

    public record InvalidData(string? EntityName, string? PropertyName) : AddRecipeError;

    private AddRecipeError() { }
}

public record GetRecipeError
{
    public record NotFound(Identifier Id) : GetRecipeError;

    private GetRecipeError() { }
}

public interface IRecipeService
{
    public Task<Result<Identifier, AddRecipeError>> AddRecipe(AddRecipe newRecipe);

    public Task<Result<Recipe, GetRecipeError>> GetRecipe(Identifier recipeId);
}
