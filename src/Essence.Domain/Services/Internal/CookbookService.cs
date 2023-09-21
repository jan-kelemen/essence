using Essence.Base.Validation;
using Essence.Base.Vocabulary;
using Essence.Domain.Model;
using Essence.Domain.Repositories;
using Essence.Domain.Vocabulary;
using System.Threading.Tasks;

namespace Essence.Domain.Services.Internal;

internal class CookbookService : ICookbookService
{
    private readonly ICookbookRepository _cookbookRepository;

    public CookbookService(ICookbookRepository cookbookRepository)
    {
        _cookbookRepository = Ensure.That.IsNotNull(cookbookRepository);
    }

    public async Task<Result<Identifier, AddRecipeError>> AddRecipe(AddRecipe newRecipe)
    {
        return await _cookbookRepository.AddRecipe(newRecipe);
    }

    public async Task<Result<Recipe, GetRecipeError>> GetRecipe(Identifier recipeId)
    {
        return await _cookbookRepository.GetRecipe(recipeId);
    }
}
