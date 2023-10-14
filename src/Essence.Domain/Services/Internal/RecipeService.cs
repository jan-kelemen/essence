using Essence.Base.Validation;
using Essence.Base.Vocabulary;
using Essence.Domain.Model;
using Essence.Domain.Repositories;
using Essence.Domain.Vocabulary;
using System.Threading.Tasks;

namespace Essence.Domain.Services.Internal;

internal class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;

    public RecipeService(IRecipeRepository cookbookRepository)
    {
        _recipeRepository = Ensure.That.IsNotNull(cookbookRepository);
    }

    public async Task<Result<Identifier, AddRecipeError>> AddRecipe(AddRecipe newRecipe)
    {
        return await _recipeRepository.AddRecipe(newRecipe);
    }

    public async Task<Result<Recipe, GetRecipeError>> GetRecipe(Identifier recipeId)
    {
        return await _recipeRepository.GetRecipe(recipeId);
    }
}
