using System;
using System.Collections.Generic;
using System.Linq;
using Essence.Base.Vocabulary;
using Essence.Domain.Model;
using Essence.Domain.Services;
using Essence.Domain.Vocabulary;
using Essence.WebAPI.Controllers;
using Essence.WebAPI.Models;
using Essence.WebAPI.Models.Endpoints;
using Essence.WebAPI.Models.Endpoints.Ingredient;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Xunit;

namespace Essence.WebAPI.Test.Unit.Controllers;

public class IngredientControllerTests
{
    [Fact]
    public void Constructor_ValidatesInputArguments()
    {
        Assert.Throws<ArgumentNullException>(() => new IngredientController(null!));
    }

    [Fact]
    public async void GetIngredient_ReturnsOk_GivenExistingIngredientId()
    {
        var identifierDto = "Id";
        var existingIngredient = new Ingredient(
            new(new(identifierDto), "Name"),
            "Summary",
            "Description");
        var ingredientService = Substitute.For<IIngredientService>();
        ingredientService.GetIngredient(new(identifierDto)).Returns(existingIngredient);

        var ingredientController = new IngredientController(ingredientService);

        var getResult = await ingredientController.GetIngredient(identifierDto);
        var ingredientDto = Assert.IsType<Ok<IngredientDto>>(getResult).Value;

        var expectedDto = new IngredientDto(identifierDto, "Name", "Summary", "Description");
        Assert.Equal(expectedDto, ingredientDto);
    }

    [Fact]
    public async void GetIngredient_ReturnsNotFound_GivenNonexistingIngredientId()
    {
        var identifierDto = "Id";
        var notFoundError = new GetIngredientError.NotFound(new(identifierDto));
        var ingredientService = Substitute.For<IIngredientService>();
        ingredientService.GetIngredient(new(identifierDto)).Returns(notFoundError);

        var ingredientController = new IngredientController(ingredientService);

        var getResult = await ingredientController.GetIngredient(identifierDto);
        Assert.IsType<NotFound>(getResult);
    }

    [Fact]
    public async void QueryIngredient_ReturnsIngredientsMatchingFilter()
    {
        var ingredientService = Substitute.For<IIngredientService>();

        var ingredients = new List<IngredientHeader>();
        var expectedIngredients = new List<IngredientHeaderDto>();
        for (int i = 0; i < 9; i++)
        {
            ingredients.Add(new(new(i.ToString()), $"Ingredient {i}"));
            expectedIngredients.Add(new(new(i.ToString()), $"Ingredient {i}"));
        }

        ingredientService.QueryIngredients("Ingredient").Returns(ingredients);

        var ingredientController = new IngredientController(ingredientService);

        var request = new QueryIngredientsRequestDto { Prefix = "Ingredient" };
        var queryResult = await ingredientController.QueryIngredients(request);
        Assert.Equal(expectedIngredients, Assert.IsType<Ok<IEnumerable<IngredientHeaderDto>>>(queryResult).Value!.ToList());
    }

    [Fact]
    public async void AddIngredient_ReturnsConflict_GivenIngredientWithAlreadyExistingName()
    {
        var ingredientService = Substitute.For<IIngredientService>();

        var conflictError = new AddIngredientError.Conflict(null);
        var newIngredient = new NewIngredient("Name", "Summary", "Description");
        ingredientService.AddIngredient(newIngredient).Returns(conflictError);

        var ingredientController = new IngredientController(ingredientService);

        var request = new AddIngredientRequestDto { Name = "Name", Summary = "Summary", Description = "Description" };
        var addResult = await ingredientController.AddIngredient(request);
        Assert.IsType<Conflict>(addResult);
    }

    [Fact]
    public async void AddIngredient_ReturnsCreated_GivenNewIngredient()
    {
        var ingredientService = Substitute.For<IIngredientService>();

        var identifier = new Identifier("Id");
        var newIngredient = new NewIngredient("Name", "Summary", "Description");
        ingredientService.AddIngredient(newIngredient).Returns(identifier);

        var ingredientController = new IngredientController(ingredientService);

        var expectedResult = new IngredientHeaderDto(identifier.Key, newIngredient.Name);

        var request = new AddIngredientRequestDto { Name = "Name", Summary = "Summary", Description = "Description" };
        var addResult = await ingredientController.AddIngredient(request);
        Assert.Equal(expectedResult, Assert.IsType<CreatedAtRoute<IngredientHeaderDto>>(addResult).Value);
    }

    [Fact]
    public async void UpdateIngredient_ReturnsConflict_GivenIngredientWithAlreadyExistingName()
    {
        var ingredientService = Substitute.For<IIngredientService>();

        var identifier = new Identifier("Id");

        var updatedIngredient = new UpdatedIngredient(identifier, "Updated Ingredient name", "Updated Summary", "Updated Description");
        var conflictError = new UpdateIngredientError.Conflict(null);
        ingredientService.UpdateIngredient(updatedIngredient).Returns(conflictError);

        var ingredientController = new IngredientController(ingredientService);

        var request = new UpdateIngredientRequestDto
        {
            Id = "Id",
            Name = "Updated Ingredient name",
            Summary = "Updated Summary",
            Description = "Updated Description"
        };
        var updateResult = await ingredientController.UpdateIngredient(request);
        Assert.IsType<Conflict>(updateResult);
    }

    [Fact]
    public async void UpdateIngredient_ReturnsNotFound_WhenNoEntitiesAreUpdated()
    {
        var ingredientService = Substitute.For<IIngredientService>();

        var identifier = new Identifier("Id");

        var updatedIngredient = new UpdatedIngredient(identifier, "Updated Ingredient name", "Updated Summary", "Updated Description");
        var notFoundError = new UpdateIngredientError.NotFound(identifier);
        ingredientService.UpdateIngredient(updatedIngredient).Returns(notFoundError);

        var ingredientController = new IngredientController(ingredientService);

        var request = new UpdateIngredientRequestDto
        {
            Id = "Id",
            Name = "Updated Ingredient name",
            Summary = "Updated Summary",
            Description = "Updated Description"
        };
        var updateResult = await ingredientController.UpdateIngredient(request);
        Assert.IsType<NotFound>(updateResult);
    }

    [Fact]
    public async void UpdateIngredient_ReturnsOk_WhenSingleEntityIsUpdated()
    {
        var ingredientService = Substitute.For<IIngredientService>();

        var identifier = new Identifier("Id");

        var updatedIngredient = new UpdatedIngredient(identifier, "Updated Ingredient name", "Updated Summary", "Updated Description");
        ingredientService.UpdateIngredient(updatedIngredient).Returns(new Option<UpdateIngredientError>());

        var ingredientController = new IngredientController(ingredientService);

        var request = new UpdateIngredientRequestDto
        {
            Id = "Id",
            Name = "Updated Ingredient name",
            Summary = "Updated Summary",
            Description = "Updated Description"
        };
        var updateResult = await ingredientController.UpdateIngredient(request);
        Assert.IsType<Ok>(updateResult);
    }

    [Fact]
    public async void DeleteIngredient_ReturnsNotFound_WhenNoEntitiesAreDeleted()
    {
        var ingredientService = Substitute.For<IIngredientService>();

        var identifier = new Identifier("Id");
        var notFoundError = new DeleteIngredientError.NotFound(identifier);
        ingredientService.DeleteIngredient(identifier).Returns(notFoundError);

        var ingredientController = new IngredientController(ingredientService);

        var deleteResult = await ingredientController.DeleteIngredient("Id");
        Assert.IsType<NotFound>(deleteResult);
    }

    [Fact]
    public async void DeleteIngredient_ReturnsOk_WhenSingleEntityIsDeleted()
    {
        var ingredientService = Substitute.For<IIngredientService>();

        var identifier = new Identifier("Id");
        ingredientService.DeleteIngredient(identifier).Returns(new Option<DeleteIngredientError>());

        var ingredientController = new IngredientController(ingredientService);

        var deleteResult = await ingredientController.DeleteIngredient("Id");
        Assert.IsType<Ok>(deleteResult);
    }
}
