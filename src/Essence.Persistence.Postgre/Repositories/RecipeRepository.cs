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
using System.Threading.Tasks;

namespace Essence.Persistence.Postgre.Repositories;

internal class RecipeRepository : IRecipeRepository
{
    private readonly IPostgreConnectionProvider _connectionProvider;

    public RecipeRepository(IPostgreConnectionProvider connectionProvider)
    {
        _connectionProvider = Ensure.That.IsNotNull(connectionProvider);
    }

    public async Task<Result<Identifier, AddRecipeError>> AddRecipe(AddRecipe newRecipe)
    {
        var commandText = "INSERT INTO recipes(name, description) VALUES (@name, @description) RETURNING id";

        var parameters = new
        {
            name = newRecipe.Name,
            description = newRecipe.Description
        };

        try
        {
            using var connection = await _connectionProvider.OpenConnection();

            return new Identifier((await connection.ExecuteScalarAsync<Guid>(commandText, parameters)).ToString());
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            return new AddRecipeError.Conflict();
        }
    }

    public async Task<Result<Recipe, GetRecipeError>> GetRecipe(Identifier recipeId)
    {
        var commandText = "SELECT id, name, description FROM recipes WHERE id = @id";

        var parameters = new
        {
            id = recipeId.ToPostgreIdentifier()
        };

        try
        {
            using var connection = await _connectionProvider.OpenConnection();

            var (id, name, description) = await connection.QuerySingleAsync<(Guid id, string name, string description)>(commandText, parameters);

            return new Recipe(id.ToDomainIdentifier(), name)
            {
                Description = description
            };
        }
        catch (InvalidOperationException)
        {
            return new GetRecipeError.NotFound(recipeId);
        }
    }
}
