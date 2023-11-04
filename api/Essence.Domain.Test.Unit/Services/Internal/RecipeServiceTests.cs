using System;
using System.Collections;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using Essence.Base.Vocabulary;
using Essence.Domain.Model;
using Essence.Domain.Repositories;
using Essence.Domain.Services;
using Essence.Domain.Services.Internal;
using Essence.Domain.Vocabulary;
using NSubstitute;
using Xunit;

namespace Essence.Domain.Test.Unit.Services.Internal;

public class RecipeServiceTests
{
    private readonly ImmutableArray<IngredientHeader> _ingredients = ImmutableArray.Create(
        new IngredientHeader(new("1"), "Ingredient 1"),
        new IngredientHeader(new("2"), "Ingredient 2"),
        new IngredientHeader(new("3"), "Ingredient 3"));

    [Fact]
    public void Constructor_ValidatesInputArguments()
    {
        Assert.Throws<ArgumentNullException>(() => new RecipeService(null!, null!));
    }

    [Fact]
    public async void AddRecipe_ReturnsConflict_AndResolvesConflictingRecipe_WhenConflictOccurs()
    {
        var ingredientRepository = Substitute.For<IIngredientRepository>();
        var recipeRepository = Substitute.For<IRecipeRepository>();

        var existingRecipe = new RecipeHeader(
            new Identifier("1"),
            nameof(AddRecipe_ReturnsConflict_AndResolvesConflictingRecipe_WhenConflictOccurs));

        var newRecipe = new AddRecipe(
            nameof(AddRecipe_ReturnsConflict_AndResolvesConflictingRecipe_WhenConflictOccurs),
            string.Empty,
            _ingredients.Select(i => new RecipeIngredient(i, 10)));

        recipeRepository.AddRecipe(newRecipe)
            .Returns(new RepositoryError.Conflict("Name"));
        recipeRepository.QueryRecipe(nameof(AddRecipe_ReturnsConflict_AndResolvesConflictingRecipe_WhenConflictOccurs))
            .Returns(new Option<RecipeHeader>(existingRecipe));

        var recipeService = new RecipeService(recipeRepository, ingredientRepository);

        var addResult = await recipeService.AddRecipe(newRecipe);
        var conflictResult = Assert.IsType<AddRecipeError.Conflict>(addResult.ExpectError());
        Assert.Equal(existingRecipe, conflictResult.ConflictingRecipe);
    }

    [Fact]
    public async void AddRecipe_ReturnsInvalidData_WhenInvalidDataOccurs()
    {
        var ingredientRepository = Substitute.For<IIngredientRepository>();
        var recipeRepository = Substitute.For<IRecipeRepository>();

        var newRecipe = new AddRecipe(
            nameof(AddRecipe_ReturnsInvalidData_WhenInvalidDataOccurs),
            string.Empty,
            _ingredients.Select(i => new RecipeIngredient(i, 10)));

        recipeRepository.AddRecipe(newRecipe)
            .Returns(new RepositoryError.InvalidData("Ingredient", "Amount"));

        var recipeService = new RecipeService(recipeRepository, ingredientRepository);

        var addResult = await recipeService.AddRecipe(newRecipe);
        var invalidData = Assert.IsType<AddRecipeError.InvalidData>(addResult.ExpectError());
        Assert.Equal("Ingredient", invalidData.EntityName);
        Assert.Equal("Amount", invalidData.PropertyName);
    }

    [Fact]
    public async void AddRecipe_ReturnsConflict_AndResolvesMissingIngredients_WhenUnresolvedEntitiesOccurs()
    {
        var ingredientRepository = Substitute.For<IIngredientRepository>();
        var recipeRepository = Substitute.For<IRecipeRepository>();

        var missingIngredients = ImmutableArray.Create(
            new RecipeIngredient(new IngredientHeader(new("4"), "Ingredient 4"), 4),
            new RecipeIngredient(new IngredientHeader(new("5"), "Ingredient 5"), 5));

        var allIngredients = _ingredients.Select(i => new RecipeIngredient(i, 10)).Concat(missingIngredients);

        var newRecipe = new AddRecipe(
            nameof(AddRecipe_ReturnsConflict_AndResolvesMissingIngredients_WhenUnresolvedEntitiesOccurs),
            string.Empty,
            allIngredients);

        recipeRepository.AddRecipe(newRecipe)
            .Returns(new RepositoryError.UnresolvedEntites("RecipeIngredients"));
        ingredientRepository
            .QueryIngredients(allIngredients.Select(i => i.Ingredient.Id))
            .ReturnsForAnyArgs(_ingredients);

        var recipeService = new RecipeService(recipeRepository, ingredientRepository);

        var addResult = await recipeService.AddRecipe(newRecipe);
        var unknownIngredients = Assert.IsType<AddRecipeError.UnknownIngredients>(addResult.ExpectError());
        Assert.Equal(missingIngredients, unknownIngredients.Ingredients);
    }

    [Fact]
    public async void AddRecipe_Works()
    {
        var ingredientRepository = Substitute.For<IIngredientRepository>();
        var recipeRepository = Substitute.For<IRecipeRepository>();

        var identifier = new Identifier("NEW");

        var newRecipe = new AddRecipe(
            nameof(AddRecipe_ReturnsInvalidData_WhenInvalidDataOccurs),
            string.Empty,
            _ingredients.Select(i => new RecipeIngredient(i, 10)));

        recipeRepository.AddRecipe(newRecipe).Returns(identifier);

        var recipeService = new RecipeService(recipeRepository, ingredientRepository);

        var addResult = await recipeService.AddRecipe(newRecipe);
        Assert.Equal(identifier, addResult.Expect());
    }

    [Fact]
    public async void GetRecipe_ReturnsNotFound_GivenNonexistingRecipeId()
    {
        var ingredientRepository = Substitute.For<IIngredientRepository>();
        var recipeRepository = Substitute.For<IRecipeRepository>();

        var identifier = new Identifier("NEW");

        var newRecipe = new AddRecipe(
            nameof(GetRecipe_ReturnsNotFound_GivenNonexistingRecipeId),
            string.Empty,
            _ingredients.Select(i => new RecipeIngredient(i, 10)));

        recipeRepository.GetRecipe(identifier).Returns(new RepositoryError.NotFound(identifier));

        var recipeService = new RecipeService(recipeRepository, ingredientRepository);

        var getResult = await recipeService.GetRecipe(identifier);
        var notFound = Assert.IsType<GetRecipeError.NotFound>(getResult.ExpectError());
        Assert.Equal(identifier, notFound.Id);
    }

    [Fact]
    public async void GetRecipe_Works()
    {
        var ingredientRepository = Substitute.For<IIngredientRepository>();
        var recipeRepository = Substitute.For<IRecipeRepository>();

        var identifier = new Identifier("EXISTING");

        var recipe = new Recipe(new(identifier, nameof(GetRecipe_Works)))
        {
            Description = string.Empty,
            Ingredients = _ingredients.Select(i => new RecipeIngredient(i, 10)),
        };

        recipeRepository.GetRecipe(identifier).Returns(recipe);

        var recipeService = new RecipeService(recipeRepository, ingredientRepository);

        var getResult = await recipeService.GetRecipe(identifier);
        var loadedRecipe = getResult.Expect();
        Assert.Equal(identifier, loadedRecipe.Id);
        Assert.Equal(nameof(GetRecipe_Works), loadedRecipe.Name);
    }
}
