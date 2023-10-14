using Essence.Base.Vocabulary;
using Essence.Domain.Model;
using Essence.Domain.Services;
using Essence.Domain.Vocabulary;
using Essence.WebAPI.Controllers;
using Essence.WebAPI.Models.Endpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Essence.WebAPI.Test.Unit.Controllers;

public class RecipeControllerTests
{
    [Fact]
    public void Constructor_ValidatesInputParameters()
    {
        Assert.Throws<ArgumentNullException>(() => { new RecipeController(null!); });
    }

    [Fact]
    public async Task AddRecipe_ReturnsHttpCreated_ForCreatedRecipe()
    {
        var recipeService = Substitute.For<IRecipeService>();

        recipeService
            .AddRecipe(Arg.Any<AddRecipe>())
            .Returns(new Result<Identifier, AddRecipeError>(new Identifier("Id")));

        var controller = new RecipeController(recipeService);

        var request = new AddRecipeRequestDto { Name = "Name" };
        var response = await controller.AddRecipe(request);

        var expectedResponse = new AddRecipeResponseDto { Id = "Id" };

        var createdResponse = Assert.IsType<CreatedAtRoute<AddRecipeResponseDto>>(response);
        Assert.Equal(expectedResponse, createdResponse.Value);
    }

    [Fact]
    public async Task AddRecipe_ReturnsHttpConflict_ForConflictingRecipe()
    {
        var recipeService = Substitute.For<IRecipeService>();

        recipeService
            .AddRecipe(Arg.Any<AddRecipe>())
            .Returns(new Result<Identifier, AddRecipeError>(new AddRecipeError.Conflict()));

        var controller = new RecipeController(recipeService);

        var request = new AddRecipeRequestDto { Name = "Name" };
        var response = await controller.AddRecipe(request);
        
        Assert.IsType<Conflict>(response);
    }

    [Fact]
    public async Task GetRecipe_ReturnsHttpNotFound_WhenRecipeIsNotFound()
    {
        var recipeService = Substitute.For<IRecipeService>();

        recipeService
            .GetRecipe(new("Id"))
            .Returns(new Result<Recipe, GetRecipeError>(new GetRecipeError.NotFound(new("Id"))));

        var controller = new RecipeController(recipeService);

        var response = await controller.GetRecipe("Id");
        Assert.IsType<NotFound>(response);
    }

    [Fact]
    public async Task GetRecipe_ReturnHttpOk_WhenRecipeIsFound()
    {
        var recipeService = Substitute.For<IRecipeService>();

        recipeService
            .GetRecipe(new("Id"))
            .Returns(new Result<Recipe, GetRecipeError>(new Recipe(new("Id"), "Existing")));

        var controller = new RecipeController(recipeService);

        var response = await controller.GetRecipe("Id");
        Assert.IsType<Ok>(response);
    }
}
