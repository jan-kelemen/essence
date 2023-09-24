using Essence.Domain.Model;
using Essence.Domain.Repositories;
using Essence.Domain.Services;
using Essence.Domain.Vocabulary;
using Essence.Persistence.Postgre.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Essence.Persistence.Postgre.Test.Integration.Repositories;

[Collection(nameof(PostgreCollection))]
public class IngredientRepositoryTests
{
    private readonly IPostgreConnectionProvider _connectionProvider;

    public IngredientRepositoryTests(PostgreFixture postgreFixture)
    {
        _connectionProvider = postgreFixture.ConnectionProvider;
    }

    [Fact]
    public async void GetIngredient_ReturnsNotFound_GivenNonexistingIdentifier()
    {
        var ingredientRepository = new IngredientRepository(_connectionProvider);

        var searchedIdentifier = new Identifier(Guid.NewGuid().ToString());

        var getResult = await ingredientRepository.GetIngredient(searchedIdentifier);

        var notFoundError = Assert.IsType<RepositoryError.NotFound>(getResult.ExpectError());
        Assert.Equal(searchedIdentifier, notFoundError.Id);
    }

    [Fact]
    public async void QueryIngredients_ReturnsAllIngredients_MatchingFilter()
    {
        var ingredientRepository = new IngredientRepository(_connectionProvider);

        var expectedHeaders = new List<IngredientHeader>();

        var prefix = $"{nameof(QueryIngredients_ReturnsAllIngredients_MatchingFilter)}_{Guid.NewGuid()}";
        for (var i = 0; i < 9; i++)
        {
            var name = $"{prefix}_{i}";

            var ingredient = new NewIngredient($"{prefix}_{i}", null, null);
            var addResult = await ingredientRepository.AddIngredient(ingredient);

            expectedHeaders.Add(new (addResult.Expect(), name));
        }

        var queryResult = (await ingredientRepository.QueryIngredients(prefix)).Expect()
            .OrderBy(i => i.Name)
            .ToList();

        Assert.Equal(expectedHeaders, queryResult);
    }

    [Fact]
    public async void AddIngredient_ReturnsIdentifier_GivenNewIngredient()
    {
        var ingredientRepository = new IngredientRepository(_connectionProvider);

        var newIngredient = new NewIngredient(Guid.NewGuid().ToString(), null, null);

        var addResult = await ingredientRepository.AddIngredient(newIngredient);

        Assert.NotEmpty(addResult.Expect().Key);
    }

    [Fact]
    public async void AddIngredient_ReturnsConflictError_GivenIngredientWithAlreadyExistingName()
    {
        var ingredientRepository = new IngredientRepository(_connectionProvider);

        var newIngredient = new NewIngredient(Guid.NewGuid().ToString(), "Initial summary", "Initial description");

        var addResult = await ingredientRepository.AddIngredient(newIngredient);
        Assert.NotEmpty(addResult.Expect().Key);

        var ingridientOfSameName = newIngredient with { Summary = "New summary", Description = "New description" };

        var secondResult = await ingredientRepository.AddIngredient(ingridientOfSameName);
        var conflictResult = Assert.IsType<RepositoryError.Conflict>(secondResult.ExpectError());
        Assert.Equal("Name", conflictResult.Property);
    }

    [Fact]
    public async void UpdateIngredient_ReturnsZeroAffectedRows_GivenNonexistingIngredient()
    {
        var ingredientRepository = new IngredientRepository(_connectionProvider);

        var ingredient = new Ingredient(
            new(new(Guid.NewGuid().ToString()), Guid.NewGuid().ToString()),
            "Initial summary",
            "Initial description");

        var updateResult = await ingredientRepository.UpdateIngredient(ingredient);
        Assert.Equal(0, updateResult.Expect());
    }

    [Fact]
    public async void UpdateIngredient_ReturnsOneAffectedRow_GivenExistingIngredient()
    {
        var ingredientRepository = new IngredientRepository(_connectionProvider);

        var newIngredient = new NewIngredient(
            Guid.NewGuid().ToString(),
            "Initial summary",
            "Initial description");

        var addResult = await ingredientRepository.AddIngredient(newIngredient);
        Assert.NotEmpty(addResult.Expect().Key);

        var updatedIngredient = new Ingredient(
            new(addResult.Expect(), newIngredient.Name),
            "Updated summary",
            "Updated summary");

        var updateResult = await ingredientRepository.UpdateIngredient(updatedIngredient);
        Assert.Equal(1, updateResult.Expect());
    }

    [Fact]
    public async void UpdateIngredient_ReturnsConflictError_GivenUpdatedIngredientOfExistingName()
    {
        var ingredientRepository = new IngredientRepository(_connectionProvider);

        var firstIngredient = new NewIngredient(
            Guid.NewGuid().ToString(),
            "Summary",
            "Description");
        var addFirstResult = ingredientRepository.AddIngredient(firstIngredient);

        var secondIngredient = new NewIngredient(
            Guid.NewGuid().ToString(),
            "Summary",
            "Description");
        var addSecondResult = ingredientRepository.AddIngredient(secondIngredient);

        Task.WaitAll(addFirstResult, addSecondResult);

        var updatedWithSameNameAsFirst = new Ingredient(
            new(addSecondResult.Result.Expect(), firstIngredient.Name),
            "Updated summary",
            "Updated summary");
        var updateResult = await ingredientRepository.UpdateIngredient(updatedWithSameNameAsFirst);

        var conflictError = Assert.IsType<RepositoryError.Conflict>(updateResult.ExpectError());
        Assert.Equal("Name", conflictError.Property);
    }

    [Fact]
    public async void DeleteIngredient_ReturnsZeroAffectedRows_GivenNonexistingIngredient()
    {
        var ingredientRepository = new IngredientRepository(_connectionProvider);

        var deleteResult = await ingredientRepository.DeleteIngredient(new(Guid.NewGuid().ToString()));
        Assert.Equal(0, deleteResult.Expect());
    }

    [Fact]
    public async void DeleteIngredient_ReturnsOneAffectedRow_GivenExistingIngredient()
    {
        var ingredientRepository = new IngredientRepository(_connectionProvider);

        var newIngredient = new NewIngredient(
            Guid.NewGuid().ToString(),
            "Initial summary",
            "Initial description");

        var addResult = await ingredientRepository.AddIngredient(newIngredient);

        var deleteResult = await ingredientRepository.DeleteIngredient(addResult.Expect());
        Assert.Equal(1, deleteResult.Expect());
    }
}
