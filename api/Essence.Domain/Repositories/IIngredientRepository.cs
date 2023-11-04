using System.Collections.Generic;
using System.Threading.Tasks;
using Essence.Base.Vocabulary;
using Essence.Domain.Model;
using Essence.Domain.Services;
using Essence.Domain.Vocabulary;

namespace Essence.Domain.Repositories;

public interface IIngredientRepository
{
    public Task<Result<Ingredient, RepositoryError>> GetIngredient(Identifier ingredientId);

    public Task<Result<IEnumerable<IngredientHeader>, RepositoryError>> QueryIngredients(IEnumerable<Identifier> identifiers);

    public Task<Result<IEnumerable<IngredientHeader>, RepositoryError>> QueryIngredients(string startsWith);

    public Task<Result<Identifier, RepositoryError>> AddIngredient(NewIngredient newIngredient);

    public Task<Result<int, RepositoryError>> UpdateIngredient(Ingredient ingredient);

    public Task<Result<int, RepositoryError>> DeleteIngredient(Identifier ingredientId);
}
