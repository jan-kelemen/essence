using Essence.Base.Validation;
using Essence.Base.Vocabulary;
using Essence.Domain.Model;
using Essence.Domain.Repositories;
using Essence.Domain.Vocabulary;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Essence.Domain.Services.Internal;

internal class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IIngredientRepository _ingredientRepository;

    public RecipeService(IRecipeRepository recipeRepository, IIngredientRepository ingredientRepository)
    {
        _recipeRepository = Ensure.That.IsNotNull(recipeRepository);
        _ingredientRepository = Ensure.That.IsNotNull(ingredientRepository);
    }

    public async Task<Result<Identifier, AddRecipeError>> AddRecipe(AddRecipe newRecipe)
    {
        var addRecipeResult = await _recipeRepository.AddRecipe(newRecipe);

        if (addRecipeResult.ExpectError(out var error))
        {
            return error switch
            {
                RepositoryError.Conflict c => (await _recipeRepository.QueryRecipe(newRecipe.Name))
                    .MapOr(conflictingOption => new AddRecipeError.Conflict(conflictingOption.HasValue() ? conflictingOption.Value() : null),
                        new AddRecipeError.Conflict(null)),
                RepositoryError.InvalidData d => new AddRecipeError.InvalidData(d.EntityType, d.Property),
                RepositoryError.UnresolvedEntites d => new AddRecipeError.UnknownIngredients(await ResolveMissingIngredients(newRecipe.Ingredients.ToList())),
                _ => throw new NotImplementedException()
            };
        }

        return addRecipeResult.Expect();
    }

    public async Task<Result<Recipe, GetRecipeError>> GetRecipe(Identifier recipeId)
    {
        return (await _recipeRepository.GetRecipe(recipeId)).MapError<GetRecipeError>(e => e switch
        {
            RepositoryError.NotFound nf => new GetRecipeError.NotFound(nf.Id),
            _ => throw new NotImplementedException()
        });
    }

    private async Task<IEnumerable<RecipeIngredient>> ResolveMissingIngredients(ICollection<RecipeIngredient> requested)
    {
        var queryResult = await _ingredientRepository.QueryIngredients(requested.Select(i => i.Ingredient.Id));

        if (queryResult.ExpectError(out var error)) { return ImmutableArray<RecipeIngredient>.Empty; }

        return requested.ExceptBy(queryResult.Expect(), x => x.Ingredient).ToList();
    }
}
