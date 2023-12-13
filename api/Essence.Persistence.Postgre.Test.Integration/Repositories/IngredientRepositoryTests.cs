using Essence.Domain.Model;
using Essence.Domain.Repositories;
using Essence.Domain.Services;
using Essence.Domain.Vocabulary;
using Essence.Persistence.Postgre.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Essence.Persistence.Postgre.Test.Integration.Repositories;

[Collection(nameof(PostgreCollection))]
public class IngredientRepositoryTests
{
    private readonly IPostgreConnectionProvider _connectionProvider;

    private readonly IIngredientRepository _repository;

    public IngredientRepositoryTests(PostgreFixture postgreFixture)
    {
        _connectionProvider = postgreFixture.ConnectionProvider;

        _repository = new IngredientRepository(_connectionProvider);
    }

    [Fact]
    public async void GetIngredient_ReturnsNotFound_GivenNonexistingIdentifier()
    {
        var searchedIdentifier = new Identifier(Guid.NewGuid().ToString());

        var getResult = await _repository.GetIngredient(searchedIdentifier);

        var notFoundError = Assert.IsType<RepositoryError.NotFound>(getResult.ExpectError());
        Assert.Equal(searchedIdentifier, notFoundError.Id);
    }

    [Fact]
    public async void QueryIngredients_ReturnsAllIngredients_MatchingFilter()
    {
        var namePrefix = $"{nameof(QueryIngredients_ReturnsAllIngredients_MatchingFilter)}_{Guid.NewGuid()}";

        var expectedHeaders = await DataGenerator.GenerateIngredients(_repository, namePrefix, 9); 

        var queryResult = (await _repository.QueryIngredients(namePrefix)).Expect()
            .OrderBy(i => i.Name)
            .ToList();

        Assert.Equal(expectedHeaders, queryResult);
    }

    [Fact]
    public async void QueryIngredients_ReturnsAllIngredients_WithMatchingIdentifier()
    {
        var namePrefix = $"{nameof(QueryIngredients_ReturnsAllIngredients_WithMatchingIdentifier)}_{Guid.NewGuid()}";

        var expectedHeaders = await DataGenerator.GenerateIngredients(_repository, namePrefix, 5);

        var idFilter = expectedHeaders.Select(i => i.Id);

        var queryResult = (await _repository.QueryIngredients(idFilter)).Expect()
            .OrderBy(i => i.Name)
            .ToList();

        Assert.Equal(expectedHeaders, queryResult);
    }

    [Fact]
    public async void QueryIngredients_Succeeds_WithNonexistingIdentifier()
    {
        var idFilter = new[] { new Identifier(Guid.NewGuid().ToString()) };

        var queryResult = (await _repository.QueryIngredients(idFilter)).Expect();

        Assert.Empty(queryResult);
    }

    [Fact]
    public async void AddIngredient_ReturnsIdentifier_GivenNewIngredient()
    {
        var newIngredient = new NewIngredient(Guid.NewGuid().ToString(), null, null);

        var addResult = await _repository.AddIngredient(newIngredient);

        Assert.NotEmpty(addResult.Expect().Key);
    }

    [Fact]
    public async void AddIngredient_ReturnsConflictError_GivenIngredientWithAlreadyExistingName()
    {
        var newIngredient = new NewIngredient(
            Guid.NewGuid().ToString(),
            "Initial summary",
            "Initial description");

        var addResult = await _repository.AddIngredient(newIngredient);
        Assert.NotEmpty(addResult.Expect().Key);

        var ingridientOfSameName = newIngredient with
        {
            Summary = "New summary",
            Description = "New description"
        };

        var secondResult = await _repository.AddIngredient(ingridientOfSameName);
        var conflictResult = Assert.IsType<RepositoryError.Conflict>(secondResult.ExpectError());
        Assert.Equal("Name", conflictResult.Property);
    }

    [Fact]
    public async void UpdateIngredient_ReturnsZeroAffectedRows_GivenNonexistingIngredient()
    {
        var ingredient = new Ingredient(
            new(new(Guid.NewGuid().ToString()), Guid.NewGuid().ToString()),
            "Initial summary",
            "Initial description");

        var updateResult = await _repository.UpdateIngredient(ingredient);
        Assert.Equal(0, updateResult.Expect());
    }

    [Fact]
    public async void UpdateIngredient_ReturnsOneAffectedRow_GivenExistingIngredient()
    {
        var newIngredient = new NewIngredient(
            Guid.NewGuid().ToString(),
            "Initial summary",
            "Initial description");

        var addResult = await _repository.AddIngredient(newIngredient);
        Assert.NotEmpty(addResult.Expect().Key);

        var updatedIngredient = new Ingredient(
            new(addResult.Expect(), newIngredient.Name),
            "Updated summary",
            "Updated summary");

        var updateResult = await _repository.UpdateIngredient(updatedIngredient);
        Assert.Equal(1, updateResult.Expect());
    }

    [Fact]
    public async void UpdateIngredient_ReturnsConflictError_GivenUpdatedIngredientOfExistingName()
    {
        var firstIngredient = new NewIngredient(
            Guid.NewGuid().ToString(),
            "Summary",
            "Description");
        var addFirstResult = _repository.AddIngredient(firstIngredient);

        var secondIngredient = new NewIngredient(
            Guid.NewGuid().ToString(),
            "Summary",
            "Description");
        var addSecondResult = _repository.AddIngredient(secondIngredient);

        await Task.WhenAll(addFirstResult, addSecondResult);

        var updatedWithSameNameAsFirst = new Ingredient(
            new(addSecondResult.Result.Expect(), firstIngredient.Name),
            "Updated summary",
            "Updated summary");
        var updateResult = await _repository.UpdateIngredient(updatedWithSameNameAsFirst);

        var conflictError = Assert.IsType<RepositoryError.Conflict>(updateResult.ExpectError());
        Assert.Equal("Name", conflictError.Property);
    }

    [Fact]
    public async void DeleteIngredient_ReturnsZeroAffectedRows_GivenNonexistingIngredient()
    {
        var deleteResult = await _repository.DeleteIngredient(new(Guid.NewGuid().ToString()));
        Assert.Equal(0, deleteResult.Expect());
    }

    [Fact]
    public async void DeleteIngredient_ReturnsOneAffectedRow_GivenExistingIngredient()
    {
        var newIngredient = new NewIngredient(
            Guid.NewGuid().ToString(),
            "Initial summary",
            "Initial description");

        var addResult = await _repository.AddIngredient(newIngredient);

        var deleteResult = await _repository.DeleteIngredient(addResult.Expect());
        Assert.Equal(1, deleteResult.Expect());
    }
}
