using Essence.Persistence.Postgre.Test.Integration;
using Essence.WebAPI.Models;
using Essence.WebAPI.Models.Endpoints;
using Essence.WebAPI.Models.Endpoints.Recipe;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace Essence.WebAPI.Test.Integration.Controllers;

public class RecipeControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    private static JsonObject IngredientDto() => new()
    {
        { "Ingredient", new JsonObject { { "Id", "123" }, { "Name", "Abc123" } } },
        { "Amount", 5 }
    };

    public RecipeControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Theory, MemberData(nameof(AddRecipe_RequestsWithInvalidValues))]
    public async void AddRecipe_DoesNotAcceptMalformedRequests(JsonObject request, string property)
    {
        var client = _factory.CreateClient();

        var response = await client.PutAsJsonAsync("api/Recipe/AddRecipe", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var details = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(details);
        Assert.Contains(property, details.Errors);
    }

    public static IEnumerable<object[]> AddRecipe_RequestsWithInvalidValues => new List<object[]>
    {
        new object[] { new JsonObject { { "Description", string.Empty }, { "Ingredients", new JsonArray { IngredientDto() } } }, "$" },
        new object[] { new JsonObject { { "Name", null }, { "Description", string.Empty }, { "Ingredients", new JsonArray { IngredientDto() } } }, "Name" },
        new object[] { new JsonObject { { "Name", string.Empty }, { "Description", string.Empty }, { "Ingredients", new JsonArray { IngredientDto() } } }, "Name" },
        new object[] { new JsonObject { { "Name", " " }, { "Description", string.Empty }, { "Ingredients", new JsonArray { IngredientDto() } } }, "Name" },

        new object[] { new JsonObject { { "Name", "A" }, { "Description", null }, { "Ingredients", new JsonArray { IngredientDto() } } }, "Description" },

        new object[] { new JsonObject { { "Name", "A" }, { "Description", string.Empty } }, "$" },
        new object[] { new JsonObject { { "Name", "A" }, { "Description", string.Empty }, { "Ingredients", new JsonArray { } } }, "Ingredients" },
    };

    [Fact]
    public async void AddRecipe_Works()
    {
        var client = _factory.CreateClient();

        var ingredients = (await DataGenerator.GenerateIngredients(client, $"{nameof(AddRecipe_Works)}_{Guid.NewGuid()}", 1))
            .Select(i => new RecipeIngredientDto(i, 5));

        var request = new AddRecipeRequestDto
        {
            Name = $"{nameof(AddRecipe_Works)}_{Guid.NewGuid()}",
            Ingredients = ingredients
        };

        var response = await client.PutAsJsonAsync("api/Recipe/AddRecipe", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async void AddRecipe_ReturnsConflictForRecipeWithAlreadyExistingName()
    {
        var client = _factory.CreateClient();

        var ingredients = (await DataGenerator.GenerateIngredients(client, $"{nameof(AddRecipe_ReturnsConflictForRecipeWithAlreadyExistingName)}_{Guid.NewGuid()}", 1))
            .Select(i => new RecipeIngredientDto(i, 5));

        var request = new AddRecipeRequestDto
        {
            Name = $"{nameof(AddRecipe_ReturnsConflictForRecipeWithAlreadyExistingName)}_{Guid.NewGuid()}",
            Ingredients = ingredients
        };

        var create = await client.PutAsJsonAsync("api/Recipe/AddRecipe", request);
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var conflict = await client.PutAsJsonAsync("api/Recipe/AddRecipe", request);
        Assert.Equal(HttpStatusCode.Conflict, conflict.StatusCode);
    }

    [Theory]
    [InlineData("6a318044-95d3-40b9-a2fc-8fef278efee2")]
    public async void GetRecipe_ReturnsNotFoundForNonexistingId(string id)
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync($"api/Recipe/GetRecipe/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async void GetRecipe_Works()
    {
        var client = _factory.CreateClient();

        var ingredients = (await DataGenerator.GenerateIngredients(client, $"{nameof(GetRecipe_Works)}_{Guid.NewGuid()}", 1))
            .Select(i => new RecipeIngredientDto(i, 5));

        var createRequest = new AddRecipeRequestDto
        {
            Name = Guid.NewGuid().ToString(),
            Ingredients = ingredients
        };
        var createResponse = await client.PutAsJsonAsync("api/Recipe/AddRecipe", createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var response = await client.GetAsync(createResponse.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
