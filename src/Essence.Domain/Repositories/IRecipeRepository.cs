using Essence.Base.Vocabulary;
using Essence.Domain.Model;
using Essence.Domain.Services;
using Essence.Domain.Vocabulary;
using System.Threading.Tasks;

namespace Essence.Domain.Repositories
{
    public interface IRecipeRepository
    {
        public Task<Result<Identifier, AddRecipeError>> AddRecipe(AddRecipe newRecipe);

        public Task<Result<Recipe, GetRecipeError>> GetRecipe(Identifier recipeId);
    }
}
