using Dapper;
using Essence.Base.Validation;
using Essence.Base.Vocabulary;
using Essence.Domain.Model;
using Essence.Domain.Repositories;
using Essence.Domain.Services;
using Essence.Domain.Vocabulary;
using Essence.Persistence.Postgre.Utility;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Essence.Persistence.Postgre.Repositories;

internal class RecipeRepository : IRecipeRepository
{
    private record struct RecipeIngredientEntry(Guid RecipeId, Guid IngredientId, int Amount);

    private readonly IPostgreConnectionProvider _connectionProvider;

    public RecipeRepository(IPostgreConnectionProvider connectionProvider)
    {
        _connectionProvider = Ensure.That.IsNotNull(connectionProvider);
    }

    public async Task<Result<Identifier, RepositoryError>> AddRecipe(AddRecipe newRecipe)
    {
        var insertIngredients = "INSERT INTO recipe_ingredient(recipe_id, ingredient_id, amount) VALUES (@RecipeId, @IngredientId, @Amount)";

        var insertRecipe = "INSERT INTO recipes(name, description) VALUES (@name, @description) RETURNING id";

        try
        {
            using var connection = await _connectionProvider.OpenConnection();

            using var transaction = connection.BeginTransaction();

            var recipe = new
            {
                name = newRecipe.Name,
                description = newRecipe.Description
            };
            var recipeId = await connection.ExecuteScalarAsync<Guid>(insertRecipe, recipe);


            var recipeIngredients = newRecipe.Ingredients
                .Select(i => new RecipeIngredientEntry(recipeId, i.Ingredient.Id.ToPostgreIdentifier(), (int)i.Amount))
                .ToArray();

            await connection.ExecuteAsync(insertIngredients, recipeIngredients);

            transaction.Commit();

            return recipeId.ToDomainIdentifier();
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            return new RepositoryError.Conflict(nameof(Domain.Services.AddRecipe.Name));
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.ForeignKeyViolation)
        {
            return new RepositoryError.UnresolvedEntites(nameof(Ingredient));
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.CheckViolation)
        {
            return new RepositoryError.InvalidData(nameof(RecipeIngredient), nameof(RecipeIngredient.Amount));
        }
    }

    public async Task<Result<Recipe, RepositoryError>> GetRecipe(Identifier recipeId)
    {
        var queryRecipe = "SELECT id, name, description FROM recipes WHERE id = @id";
        var queryIngredients = "SELECT recipe_id, ingredient_id, name, amount FROM recipe_ingredient INNER JOIN ingredients ON ingredient_id = ingredients.id WHERE recipe_id = @id";

        var parameters = new
        {
            id = recipeId.ToPostgreIdentifier()
        };

        try
        {
            using var connection = await _connectionProvider.OpenConnection();

            var ingredients = (await connection.QueryAsync<(Guid recipeId, Guid ingredientId, string name, int amount)>(queryIngredients, parameters))
                .Select(i => new RecipeIngredient(new(i.ingredientId.ToDomainIdentifier(), i.name), (uint)i.amount));

            var (id, name, description) = await connection.QuerySingleAsync<(Guid id, string name, string description)>(queryRecipe, parameters);

            return new Recipe(new(id.ToDomainIdentifier(), name))
            {
                Description = description,
                Ingredients = ingredients
            };
        }
        catch (InvalidOperationException)
        {
            return new RepositoryError.NotFound(recipeId);
        }
    }

    public async Task<Result<Option<RecipeHeader>, RepositoryError>> QueryRecipe(string name)
    {
        var commandText = "SELECT id, name FROM recipes WHERE name = @name";

        var parameters = new { name };

        using var connection = await _connectionProvider.OpenConnection();

        var queryResult = await connection.QuerySingleOrDefaultAsync<(Guid id, string recipeName)?>(commandText, parameters);

        return queryResult.HasValue ?
            new Option<RecipeHeader>() :
            new Option<RecipeHeader>(new(queryResult.Value.id.ToDomainIdentifier(), queryResult.Value.recipeName));
    }
}
