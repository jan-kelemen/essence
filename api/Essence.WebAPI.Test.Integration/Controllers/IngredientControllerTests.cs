using Essence.WebAPI.Models;
using Essence.WebAPI.Models.Endpoints;
using Essence.WebAPI.Models.Endpoints.Ingredient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace Essence.WebAPI.Test.Integration.Controllers;

public class IngredientControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public IngredientControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Theory, MemberData(nameof(AddIngredient_RequestsWithInvalidValues))]
    public async void AddIngredient_DoesNotAcceptMalformedRequests(JsonObject request, string property)
    {
        var client = _factory.CreateClient();

        var response = await client.PutAsJsonAsync("api/Ingredient/AddIngredient", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var details = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(details);
        Assert.Contains(property, details.Errors);
    }

    public static IEnumerable<object[]> AddIngredient_RequestsWithInvalidValues => new List<object[]>
    {
        new object[] { new JsonObject { { "Name", null } }, "Name" },
        new object[] { new JsonObject { { "Name", string.Empty }}, "Name" },
        new object[] { new JsonObject { { "Name", " " } }, "Name" },
    };

    [Fact]
    public async void AddIngredient_Works()
    {
        var client = _factory.CreateClient();

        var request = new AddIngredientRequestDto
        {
            Name = Guid.NewGuid().ToString(),
            Summary = "Summary",
            Description = "Description",
        };

        var response = await client.PutAsJsonAsync("api/Ingredient/AddIngredient", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Theory, MemberData(nameof(UpdateIngredient_RequestsWithInvalidValues))]
    public async void UpdateIngredient_DoesNotAcceptMalformedRequests(JsonObject request, string property)
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("api/Ingredient/UpdateIngredient", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var details = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(details);
        Assert.Contains(property, details.Errors);
    }

    public static IEnumerable<object[]> UpdateIngredient_RequestsWithInvalidValues => new List<object[]>
    {
        new object[] { new JsonObject { { "Id", null }, { "Name", "OK" } }, "Id" },
        new object[] { new JsonObject { { "Id", string.Empty }, { "Name", "OK" } }, "Id" },
        new object[] { new JsonObject { { "Id", " " }, { "Name", "OK" } }, "Id" },
        new object[] { new JsonObject { { "Id", "OK" }, { "Name", null } }, "Name" },
        new object[] { new JsonObject { { "Id", "OK" }, { "Name", string.Empty } }, "Name" },
        new object[] { new JsonObject { { "Id", "OK" }, { "Name", " " } }, "Name" },
    };

    [Fact]
    public async void AddedIngredient_CanBeRetrived()
    {
        var client = _factory.CreateClient();

        var request = new AddIngredientRequestDto
        {
            Name = Guid.NewGuid().ToString(),
            Summary = "Summary",
            Description = "Description",
        };

        var addResponse = await client.PutAsJsonAsync("api/Ingredient/AddIngredient", request);
        Assert.Equal(HttpStatusCode.Created, addResponse.StatusCode);
        var ingredientHeader = await addResponse.Content.ReadFromJsonAsync<IngredientHeaderDto>();

        var getResponse = await client.GetAsync(addResponse.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var expectedIngredientDto = new IngredientDto
        {
            Id = ingredientHeader.Id,
            Name = ingredientHeader.Name,
            Summary = "Summary",
            Description = "Description",
        };
        var ingredient = await getResponse.Content.ReadFromJsonAsync<IngredientDto>();
        Assert.Equal(expectedIngredientDto, ingredient);
    }

    [Fact]
    public async void AddedIngredient_CanBeUpdated()
    {
        var client = _factory.CreateClient();

        var request = new AddIngredientRequestDto
        {
            Name = Guid.NewGuid().ToString(),
            Summary = "Summary",
            Description = "Description",
        };

        var addResponse = await client.PutAsJsonAsync("api/Ingredient/AddIngredient", request);
        Assert.Equal(HttpStatusCode.Created, addResponse.StatusCode);
        var ingredientHeader = await addResponse.Content.ReadFromJsonAsync<IngredientHeaderDto>();

        var updateRequest = new UpdateIngredientRequestDto
        {
            Id = ingredientHeader.Id,
            Name = $"Updated {ingredientHeader.Name}",
            Summary = "Updated Summary",
            Description = "Updated Description",
        };
        var updateResponse = await client.PostAsJsonAsync("api/Ingredient/UpdateIngredient", updateRequest);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var getResponse = await client.GetAsync(addResponse.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var expectedIngredientDto = new IngredientDto
        {
            Id = ingredientHeader.Id,
            Name = $"Updated {ingredientHeader.Name}",
            Summary = "Updated Summary",
            Description = "Updated Description",
        };
        var ingredient = await getResponse.Content.ReadFromJsonAsync<IngredientDto>();
        Assert.Equal(expectedIngredientDto, ingredient);
    }

    [Fact]
    public async void AddedIngredient_CanBeDeleted()
    {
        var client = _factory.CreateClient();

        var request = new AddIngredientRequestDto
        {
            Name = Guid.NewGuid().ToString(),
            Summary = "Summary",
            Description = "Description",
        };

        var addResponse = await client.PutAsJsonAsync("api/Ingredient/AddIngredient", request);
        Assert.Equal(HttpStatusCode.Created, addResponse.StatusCode);
        var ingredientHeader = await addResponse.Content.ReadFromJsonAsync<IngredientHeaderDto>();

        var updateResponse = await client.DeleteAsync($"api/Ingredient/DeleteIngredient/{ingredientHeader.Id}");
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var getResponse = await client.GetAsync($"api/Ingredient/GetIngredient/{ingredientHeader.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async void QueryIngredients_ReturnsIngredientsMatchingPrefixFilter()
    {
        var client = _factory.CreateClient();

        var prefix = Guid.NewGuid().ToString();
        var request = new AddIngredientRequestDto
        {
            Name = $"{prefix}_{nameof(QueryIngredients_ReturnsIngredientsMatchingPrefixFilter)}",
            Summary = "Summary",
            Description = "Description",
        };

        var addResponse = await client.PutAsJsonAsync("api/Ingredient/AddIngredient", request);
        Assert.Equal(HttpStatusCode.Created, addResponse.StatusCode);
        var ingredientHeader = await addResponse.Content.ReadFromJsonAsync<IngredientHeaderDto>();

        var queryResponse = await client.GetAsync($"api/Ingredient/QueryIngredients?prefix={prefix}");
        Assert.Equal(HttpStatusCode.OK, queryResponse.StatusCode);
        var matchingHeaders = await queryResponse.Content.ReadFromJsonAsync<IEnumerable<IngredientHeaderDto>>();

        Assert.NotNull(matchingHeaders);
        var queriedHeader = Assert.Single(matchingHeaders);

        Assert.Equal(ingredientHeader.Id, queriedHeader.Id);
    }
}
