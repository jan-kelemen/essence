using Essence.Domain.Model;
using Essence.Domain.Repositories;
using Essence.Domain.Services;
using Essence.Domain.Vocabulary;
using Essence.Persistence.Postgre.Repositories;
using System;
using System.Linq;
using Xunit;

namespace Essence.Persistence.Postgre.Test.Integration.Repositories;

[Collection(nameof(PostgreCollection))]
public class RecipeRepositoryTests
{
    private readonly IPostgreConnectionProvider _connectionProvider;
    private readonly IIngredientRepository _ingredientRepository;

    private readonly RecipeRepository _repository;

    public RecipeRepositoryTests(PostgreFixture postgreFixture)
    {
        _connectionProvider = postgreFixture.ConnectionProvider;
        _ingredientRepository = postgreFixture.IngredientRepository;

        _repository = postgreFixture.RecipeRepository;
    }

    [Fact]
    public async void AddRecipe_ReturnsConflict_WhenRecipeOfTheSameNameExists()
    {
        var ingredients = await DataGenerator.GenerateIngredients(
            _ingredientRepository,
            $"{nameof(AddRecipe_ReturnsConflict_WhenRecipeOfTheSameNameExists)}_{Guid.NewGuid()}",
            1);

        var usedName = Guid.NewGuid().ToString();

        var addResult = await _repository.AddRecipe(new(
            usedName,
            string.Empty,
            ingredients.Select(i => new RecipeIngredient(i, 5))));
        Assert.True(addResult.IsSuccess);

        var addRecipeResult = await _repository.AddRecipe(new(
            usedName,
            string.Empty,
            ingredients.Select(i => new RecipeIngredient(i, 5))));
        var conflict = Assert.IsType<RepositoryError.Conflict>(addRecipeResult.ExpectError());
        Assert.Equal("Name", conflict.Property);
    }

    [Fact]
    public async void AddRecipe_ReturnsUnknownIngredient_WhenRecipeIngredientIsMissing()
    {
        var ingredients = await DataGenerator.GenerateIngredients(
            _ingredientRepository,
            $"{nameof(AddRecipe_ReturnsUnknownIngredient_WhenRecipeIngredientIsMissing)}_{Guid.NewGuid()}",
            1);
        ingredients.Add(new IngredientHeader(new(Guid.NewGuid().ToString()), "missing ingredient"));

        var addResult = await _repository.AddRecipe(
            new AddRecipe(Guid.NewGuid().ToString(),
            string.Empty,
            ingredients.Select(i => new RecipeIngredient(i, 5))));
        var unkonwIngredient = Assert.IsType<RepositoryError.UnresolvedEntites>(addResult.ExpectError());
        Assert.Equal("Ingredient", unkonwIngredient.EntityType);
    }

    [Fact]
    public async void AddRecipe_Works()
    {
        var ingredients = await DataGenerator.GenerateIngredients(
            _ingredientRepository,
            $"{nameof(AddRecipe_Works)}_{Guid.NewGuid()}",
            3);

        uint amountOfIngredient = 100;
        var addResult = await _repository.AddRecipe(
            new AddRecipe(Guid.NewGuid().ToString(),
            string.Empty,
            ingredients.Select(i => new RecipeIngredient(i, amountOfIngredient += 100))));
        Assert.True(addResult.IsSuccess);
    }

    [Fact]
    public async void GetRecipe_ReturnsNotFound_GivenNonexistingIdentifier()
    {
        var searchedIdentifier = new Identifier(Guid.NewGuid().ToString());

        var recipe = await _repository.GetRecipe(searchedIdentifier);

        var notFoundError = Assert.IsType<RepositoryError.NotFound>(recipe.ExpectError());
        Assert.Equal(searchedIdentifier, notFoundError.Id);
    }

    [Fact]
    public async void GetRecipe_Works()
    {
        var ingredients = await DataGenerator.GenerateIngredients(
            _ingredientRepository,
            $"{nameof(GetRecipe_Works)}_{Guid.NewGuid()}",
            1);

        var usedName = Guid.NewGuid().ToString();

        var recipeIngredients = ingredients.Select(i => new RecipeIngredient(i, 5));
        var addResult = await _repository.AddRecipe(new(
            usedName,
            string.Empty,
            recipeIngredients));
        Assert.True(addResult.IsSuccess);

        var loadedRecipeResult = await _repository.GetRecipe(addResult.Expect());
        Assert.True(loadedRecipeResult.IsSuccess);

        var loadedRecipe = loadedRecipeResult.Expect();
        Assert.Equal(usedName, loadedRecipe.Name);
        Assert.Equal(string.Empty, loadedRecipe.Description);
        Assert.Equal(recipeIngredients, loadedRecipe.Ingredients);
    }
}
