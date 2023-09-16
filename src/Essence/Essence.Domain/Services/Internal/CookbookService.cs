﻿using Essence.Base.Vocabulary;
using Essence.Domain.Model;
using Essence.Domain.Vocabulary;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Essence.Domain.Services.Internal;

internal class CookbookService : ICookbookService
{
    private static readonly ConcurrentDictionary<Identifier, Recipe> _recipes = new();

    public Task<Result<Identifier, AddRecipeError>> AddRecipe(AddRecipe newRecipe)
    {
        var recipe = new Recipe(new(newRecipe.Name), newRecipe.Name)
        {
            Description = newRecipe.Description
        };

        if (!_recipes.TryAdd(recipe.Id, recipe))
        {
            return Task.FromResult(Result<Identifier, AddRecipeError>.FromError(new AddRecipeError.Conflict()));
        }

        return Task.FromResult(Result<Identifier, AddRecipeError>.FromSuccess(recipe.Id));
    }

    public Task<Result<Recipe, GetRecipeError>> GetRecipe(Identifier recipeId)
    {
        if (!_recipes.TryGetValue(recipeId, out var recipe))
        {
            return Task.FromResult(Result<Recipe, GetRecipeError>.FromError(new GetRecipeError.NotFound(recipeId)));
        }

        return Task.FromResult(Result<Recipe, GetRecipeError>.FromSuccess(recipe));
    }
}
