using Essence.Base.Vocabulary;
using Essence.Domain.Model;
using Essence.Domain.Services;
using Essence.Domain.Vocabulary;
using System.Threading.Tasks;

namespace Essence.Domain.Repositories;

public interface IRecipeRepository
{
    public Task<Result<Identifier, RepositoryError>> AddRecipe(AddRecipe newRecipe);

    public Task<Result<Recipe, RepositoryError>> GetRecipe(Identifier recipeId);

    public Task<Result<Option<RecipeHeader>, RepositoryError>> QueryRecipe(string name);
}
