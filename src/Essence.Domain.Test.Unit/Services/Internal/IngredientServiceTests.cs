using System;
using System.Collections.Generic;
using Essence.Domain.Model;
using Essence.Domain.Repositories;
using Essence.Domain.Services;
using Essence.Domain.Services.Internal;
using Essence.Domain.Vocabulary;
using NSubstitute;
using Xunit;

namespace Essence.Domain.Test.Unit.Services.Internal;

public class IngredientServiceTests
{
    [Fact]
    public void Constructor_ValidatesInputArguments()
    {
        Assert.Throws<ArgumentNullException>(() => new IngredientService(null!));
    }

    [Fact]
    public async void GetIngredient_ReturnsNotFound_GivenNonexistingIngredientId()
    {
        var ingredientRepository = Substitute.For<IIngredientRepository>();

        var nonexistingIdentifier = new Identifier("Id");
        var notFoundError = new RepositoryError.NotFound(nonexistingIdentifier);
        ingredientRepository.GetIngredient(nonexistingIdentifier).Returns(notFoundError);

        var ingredientService = new IngredientService(ingredientRepository);

        var getResult = await ingredientService.GetIngredient(nonexistingIdentifier);
        Assert.Equal(nonexistingIdentifier, Assert.IsType<GetIngredientError.NotFound>(getResult.ExpectError()).Id);
    }

    [Fact]
    public async void GetIngredient_ReturnsMatchingIngredient_GivenExistingId()
    {
        var ingredientRepository = Substitute.For<IIngredientRepository>();

        var ingredient = new Ingredient(
            new(new("Id"), "Ingredient name"),
            "Summary",
            "Description");
        ingredientRepository.GetIngredient(ingredient.Id).Returns(ingredient);

        var ingredientService = new IngredientService(ingredientRepository);

        var getResult = await ingredientService.GetIngredient(ingredient.Id);
        Assert.Equal(ingredient, getResult.Expect());
    }

    [Fact]
    public async void QueryIngredients_ReturnsIngredientsMatchingFilter()
    {
        var ingredientRepository = Substitute.For<IIngredientRepository>();

        var ingredients = new List<IngredientHeader>();
        for(int i = 0; i < 9; i++)
        {
            ingredients.Add(new IngredientHeader(new(i.ToString()), $"Ingredient {i}"));
        }

        ingredientRepository.QueryIngredients("Ingredient").Returns(ingredients);

        var ingredientService = new IngredientService(ingredientRepository);

        var queryResult = await ingredientService.QueryIngredients("Ingredient");
        Assert.Equal(ingredients, queryResult.Expect());
    }

    [Fact]
    public async void AddIngredient_ReturnsConflict_GivenIngredientWithAlreadyExistingName()
    {
        var ingredientRepository = Substitute.For<IIngredientRepository>();

        var newIngredient = new NewIngredient("Name", "Summary", "Description");
        var conflictError = new RepositoryError.Conflict("Name");

        ingredientRepository.AddIngredient(newIngredient).Returns(conflictError);

        var ingredientService = new IngredientService(ingredientRepository);

        var addResult = await ingredientService.AddIngredient(newIngredient);
        Assert.IsType<AddIngredientError.Conflict>(addResult.ExpectError());
    }

    [Fact]
    public async void AddIngredient_ReturnsIdentifier_GivenNewIngredient()
    {
        var ingredientRepository = Substitute.For<IIngredientRepository>();

        var newIngredient = new NewIngredient("Name", "Summary", "Description");
        var newIdentifier = new Identifier("Id");

        ingredientRepository.AddIngredient(newIngredient).Returns(newIdentifier);

        var ingredientService = new IngredientService(ingredientRepository);

        var addResult = await ingredientService.AddIngredient(newIngredient);
        Assert.Equal(newIdentifier, addResult.Expect());
    }

    [Fact]
    public async void UpdateIngredient_ReturnsNotFound_GivenNonexistingIngredientId()
    {
        var ingredientRepository = Substitute.For<IIngredientRepository>();

        var identifier = new Identifier("Id");
        var notFoundEror = new RepositoryError.NotFound(identifier);
        ingredientRepository.GetIngredient(identifier).Returns(notFoundEror);

        var ingredientService = new IngredientService(ingredientRepository);

        var request = new UpdatedIngredient(identifier, "Updated Ingredient name", "Updated Summary", "Updated Description");
        var updateResult = await ingredientService.UpdateIngredient(request);

        Assert.Equal(identifier, Assert.IsType<UpdateIngredientError.NotFound>(updateResult.Value()).Id);
    }

    [Fact]
    public async void UpdateIngredient_ReturnsConflict_GivenIngredientWithAlreadyExistingName()
    {
        var ingredientRepository = Substitute.For<IIngredientRepository>();

        var identifier = new Identifier("Id");
        var existingIngredient = new Ingredient(
            new(identifier, "Ingredient name"),
            "Summary",
            "Description");
        ingredientRepository.GetIngredient(identifier).Returns(existingIngredient);

        var updatedIngredient = Arg.Is<Ingredient>(i =>
            i.Id == identifier &&
            i.Name == "Updated Ingredient name" &&
            i.Summary == "Updated Summary" &&
            i.Description == "Updated Description");
        var conflictError = new RepositoryError.Conflict("Name");
        ingredientRepository.UpdateIngredient(updatedIngredient).Returns(conflictError);

        var ingredientService = new IngredientService(ingredientRepository);

        var request = new UpdatedIngredient(identifier, "Updated Ingredient name", "Updated Summary", "Updated Description");
        var updateResult = await ingredientService.UpdateIngredient(request);
        Assert.IsType<UpdateIngredientError.Conflict>(updateResult.Value());
    }

    [Fact]
    public async void UpdateIngredient_ReturnsNotFound_WhenNoEntitiesAreUpdated()
    {
        var ingredientRepository = Substitute.For<IIngredientRepository>();

        var identifier = new Identifier("Id");
        var existingIngredient = new Ingredient(
            new(identifier, "Ingredient name"),
            "Summary",
            "Description");
        ingredientRepository.GetIngredient(identifier).Returns(existingIngredient);

        var updatedIngredient = Arg.Is<Ingredient>(i =>
            i.Id == identifier &&
            i.Name == "Updated Ingredient name" &&
            i.Summary == "Updated Summary" &&
            i.Description == "Updated Description");
        ingredientRepository.UpdateIngredient(updatedIngredient).Returns(0);

        var ingredientService = new IngredientService(ingredientRepository);

        var request = new UpdatedIngredient(identifier, "Updated Ingredient name", "Updated Summary", "Updated Description");
        var updateResult = await ingredientService.UpdateIngredient(request);
        Assert.Equal(identifier, Assert.IsType<UpdateIngredientError.NotFound>(updateResult.Value()).Id);
    }

    [Fact]
    public async void UpdateIngredient_ReturnsNoError_WhenSingleEntityIsUpdated()
    {
        var ingredientRepository = Substitute.For<IIngredientRepository>();

        var identifier = new Identifier("Id");
        var existingIngredient = new Ingredient(
            new(identifier, "Ingredient name"),
            "Summary",
            "Description");
        ingredientRepository.GetIngredient(identifier).Returns(existingIngredient);

        var updatedIngredient = Arg.Is<Ingredient>(i =>
            i.Id == identifier &&
            i.Name == "Updated Ingredient name" &&
            i.Summary == "Updated Summary" &&
            i.Description == "Updated Description");
        ingredientRepository.UpdateIngredient(updatedIngredient).Returns(1);

        var ingredientService = new IngredientService(ingredientRepository);

        var request = new UpdatedIngredient(identifier, "Updated Ingredient name", "Updated Summary", "Updated Description");
        var updateResult = await ingredientService.UpdateIngredient(request);

        Assert.False(updateResult.HasValue);
    }

    [Fact]
    public async void DeleteIngredient_ReturnsNotFound_WhenNoEntitiesAreDeleted()
    {
        var ingredientRepository = Substitute.For<IIngredientRepository>();

        var identifier = new Identifier("Id");
        ingredientRepository.DeleteIngredient(identifier).Returns(0);

        var ingredientService = new IngredientService(ingredientRepository);

        var updateResult = await ingredientService.DeleteIngredient(identifier);
        Assert.Equal(identifier, Assert.IsType<DeleteIngredientError.NotFound>(updateResult.Value()).Id);
    }

    [Fact]
    public async void DeleteIngredient_ReturnsNoError_WhenSingleEntityIsDeleted()
    {
        var ingredientRepository = Substitute.For<IIngredientRepository>();

        var identifier = new Identifier("Id");
        ingredientRepository.DeleteIngredient(identifier).Returns(1);

        var ingredientService = new IngredientService(ingredientRepository);

        var updateResult = await ingredientService.DeleteIngredient(identifier);
        Assert.False(updateResult.HasValue);
    }
}
