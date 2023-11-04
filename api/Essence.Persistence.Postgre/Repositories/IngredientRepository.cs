using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Essence.Base.Validation;
using Essence.Base.Vocabulary;
using Essence.Domain.Model;
using Essence.Domain.Repositories;
using Essence.Domain.Services;
using Essence.Domain.Vocabulary;
using Dapper;
using Essence.Persistence.Postgre.Utility;
using Npgsql;
using System.Linq;

namespace Essence.Persistence.Postgre.Repositories;

internal class IngredientRepository : IIngredientRepository
{
    private readonly IPostgreConnectionProvider _connectionProvider;

    public IngredientRepository(IPostgreConnectionProvider connectionProvider)
    {
        _connectionProvider = Ensure.That.IsNotNull(connectionProvider);
    }

    public async Task<Result<Ingredient, RepositoryError>> GetIngredient(Identifier ingredientId)
    {
        var commandText = "SELECT id, name, summary, description FROM ingredients WHERE id = @id";

        var parameters = new { id = ingredientId.ToPostgreIdentifier() };

        try
        {
            using var connection = await _connectionProvider.OpenConnection();

            var (id, name, summary, description) =
                await connection.QuerySingleAsync<(Guid id, string name, string? summary, string? description)>(commandText, parameters);

            return new Ingredient(new(id.ToDomainIdentifier(), name), summary, description);
        }
        catch (InvalidOperationException)
        {
            return new RepositoryError.NotFound(ingredientId);
        }
    }

    public async Task<Result<IEnumerable<IngredientHeader>, RepositoryError>> QueryIngredients(string startsWith)
    {
        var commandText = "SELECT id, name FROM ingredients WHERE name LIKE @name";

        var parameters = new { name = $"{startsWith}%" };

        using var connection = await _connectionProvider.OpenConnection();

        var matchingIngredients = await connection.QueryAsync<(Guid id, string name)>(commandText, parameters);

        return new Result<IEnumerable<IngredientHeader>, RepositoryError>(
            matchingIngredients.Select(i => new IngredientHeader(i.id.ToDomainIdentifier(), i.name)));
    }

    public async Task<Result<IEnumerable<IngredientHeader>, RepositoryError>> QueryIngredients(IEnumerable<Identifier> identifiers)
    {
        var commandText = "SELECT id, name FROM ingredients WHERE id = ANY(@ids)";

        var parameters = new { ids =  identifiers.Select(PostgreIdentifierExtensions.ToPostgreIdentifier).ToArray() };

        using var connection = await _connectionProvider.OpenConnection();

        var matchingIngredients = await connection.QueryAsync<(Guid id, string name)>(commandText, parameters);

        return new Result<IEnumerable<IngredientHeader>, RepositoryError>(
            matchingIngredients.Select(i => new IngredientHeader(i.id.ToDomainIdentifier(), i.name)));
    }

    public async Task<Result<Identifier, RepositoryError>> AddIngredient(NewIngredient newIngredient)
    {
        var commandText = "INSERT INTO ingredients(name, summary, description) VALUES (@name, @summary, @description) RETURNING id";

        var parameters = new
        {
            name = newIngredient.Name,
            summary = newIngredient.Summary,
            description = newIngredient.Description
        };

        try
        {
            using var connection = await _connectionProvider.OpenConnection();

            var newId = connection.ExecuteScalar<Guid>(commandText, parameters);

            return newId.ToDomainIdentifier();
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation && ex.ConstraintName == "ingredients_name_uq")
        {
            return new RepositoryError.Conflict(nameof(NewIngredient.Name));
        }
    }

    public async Task<Result<int, RepositoryError>> UpdateIngredient(Ingredient ingredient)
    {
        var commandText = "UPDATE ingredients SET name = @name, summary = @summary, description = @description WHERE id = @id";

        var parameters = new
        {
            id = ingredient.Id.ToPostgreIdentifier(),
            name = ingredient.Name,
            summary = ingredient.Summary,
            description = ingredient.Description
        };

        try
        {
            using var connection = await _connectionProvider.OpenConnection();

            return await connection.ExecuteAsync(commandText, parameters);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation && ex.ConstraintName == "ingredients_name_uq")
        {
            return new RepositoryError.Conflict(nameof(Ingredient.Name));
        }
    }

    public async Task<Result<int, RepositoryError>> DeleteIngredient(Identifier ingredientId)
    {
        var commandText = "DELETE FROM ingredients WHERE id = @id";

        var parameters = new { id = ingredientId.ToPostgreIdentifier() };

        using var connection = await _connectionProvider.OpenConnection();

        return await connection.ExecuteAsync(commandText, parameters);
    }
}
