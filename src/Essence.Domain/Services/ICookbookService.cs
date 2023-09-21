using Essence.Base.Validation;
using Essence.Base.Vocabulary;
using Essence.Domain.Model;
using Essence.Domain.Vocabulary;
using System.Threading.Tasks;

namespace Essence.Domain.Services;

public readonly record struct AddRecipe(string Name, string Description)
{
    public string Name { get; init; } = Ensure.That.IsNotNullOrWhitespace(Name);

    public string Description { get; init; } = Ensure.That.IsNotNull(Description);
}

public record AddRecipeError
{
    public record Conflict() : AddRecipeError;

    private AddRecipeError() { }
}

public record GetRecipeError
{
    public record NotFound(Identifier Id) : GetRecipeError;

    private GetRecipeError() { }
}

public interface ICookbookService
{
    public Task<Result<Identifier, AddRecipeError>> AddRecipe(AddRecipe newRecipe);

    public Task<Result<Recipe, GetRecipeError>> GetRecipe(Identifier recipeId);
}
