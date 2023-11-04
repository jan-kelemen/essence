using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Essence.Base.Validation;
using Essence.Base.Vocabulary;
using Essence.Domain.Model;
using Essence.Domain.Repositories;
using Essence.Domain.Vocabulary;

namespace Essence.Domain.Services.Internal;

internal class IngredientService : IIngredientService
{
    private readonly IIngredientRepository _ingredientRepository;

    public IngredientService(IIngredientRepository ingredientRepository)
    {
        _ingredientRepository = Ensure.That.IsNotNull(ingredientRepository);
    }

    public async Task<Result<Ingredient, GetIngredientError>> GetIngredient(Identifier id)
    {
        return (await _ingredientRepository.GetIngredient(id)).MapError<GetIngredientError>(e => e switch
        {
            RepositoryError.NotFound nf => new GetIngredientError.NotFound(nf.Id),
            _ => throw new NotImplementedException(),
        });
    }

    public async Task<Result<IEnumerable<IngredientHeader>, QueryIngredientsError>> QueryIngredients(string filter)
    {
        return (await _ingredientRepository.QueryIngredients(filter)).MapError<QueryIngredientsError>(e => e switch
        {
            _ => throw new NotImplementedException(),
        });
    }

    public async Task<Result<Identifier, AddIngredientError>> AddIngredient(NewIngredient ingredient)
    {
        return (await _ingredientRepository.AddIngredient(ingredient)).MapError<AddIngredientError>(e => e switch
        {
            RepositoryError.Conflict c => new AddIngredientError.Conflict(null),
            _ => throw new NotImplementedException(),
        });
    }

    public async Task<Option<UpdateIngredientError>> UpdateIngredient(UpdatedIngredient ingredient)
    {
        var existingIngredientResult = (await GetIngredient(ingredient.Id))
            .MapError(e => e switch
            {
                GetIngredientError.NotFound nf => new UpdateIngredientError.NotFound(nf.Id),
                _ => throw new NotImplementedException(),
            });
        if (existingIngredientResult.ExpectError(out var notFound)) { return notFound; }

        var existingIngredient = existingIngredientResult.Expect();

        var updatedIngredient = new Ingredient(
            new(existingIngredient.Id, ingredient.Name),
            ingredient.Summary, ingredient.Description);

        return (await _ingredientRepository.UpdateIngredient(updatedIngredient)).MapOrElse(
            s => s switch
            {
                0 => new UpdateIngredientError.NotFound(ingredient.Id),
                1 => new Option<UpdateIngredientError>(),
                _ => throw new NotImplementedException(),
            },
            e => e switch
            {
                RepositoryError.Conflict c => new UpdateIngredientError.Conflict(null),
                _ => throw new NotImplementedException(),
            });
    }

    public async Task<Option<DeleteIngredientError>> DeleteIngredient(Identifier id)
    {
        return (await _ingredientRepository.DeleteIngredient(id)).MapOrElse(
            s => s switch
            {
                0 => new DeleteIngredientError.NotFound(id),
                1 => new Option<DeleteIngredientError>(),
                _ => throw new NotImplementedException(),
            },
            e => e switch
            {
                _ => throw new NotImplementedException(),
            });
    }
}
