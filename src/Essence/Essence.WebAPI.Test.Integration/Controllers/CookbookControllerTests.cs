using Essence.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace Essence.WebAPI.Test.Integration.Controllers;

public class CookbookControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CookbookControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Theory, MemberData(nameof(AddRecipe_RequestsWithInvalidValues))]
    public async void AddRecipe_DoesNotAcceptMalformedRequests(JsonObject request, string property)
    {
        var client = _factory.CreateClient();

        var response = await client.PutAsJsonAsync("api/Cookbook/AddRecipe", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var details = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(details);
        Assert.Contains(property, details.Errors);
    }

    public static IEnumerable<object[]> AddRecipe_RequestsWithInvalidValues => new List<object[]>
    {
        new object[] { new JsonObject { { "Name", null } }, "Name" },
        new object[] { new JsonObject { { "Name", string.Empty } }, "Name" },
        new object[] { new JsonObject { { "Name", " " } }, "Name" },
        new object[] { new JsonObject { { "Name", "A" }, { "Description", null } }, "Description" },
    };

    [Fact]
    public async void AddRecipe_Works()
    {
        var client = _factory.CreateClient();

        var request = new AddRecipeRequestDto
        {
            Name = Guid.NewGuid().ToString()
        };

        var response = await client.PutAsJsonAsync("api/Cookbook/AddRecipe", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async void AddRecipe_ReturnsConflictForRecipeWithAlreadyExistingName()
    {
        var client = _factory.CreateClient();

        var request = new AddRecipeRequestDto
        {
            Name = Guid.NewGuid().ToString()
        };

        var create = await client.PutAsJsonAsync("api/Cookbook/AddRecipe", request);
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var conflict = await client.PutAsJsonAsync("api/Cookbook/AddRecipe", request);
        Assert.Equal(HttpStatusCode.Conflict, conflict.StatusCode);
    }

    [Theory]
    [InlineData("6a318044-95d3-40b9-a2fc-8fef278efee2")]
    public async void GetRecipe_ReturnsNotFoundForNonexistingId(string id)
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync($"api/Cookbook/AddRecipe/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async void GetRecipe_Works()
    {
        var client = _factory.CreateClient();

        var createRequest = new AddRecipeRequestDto
        {
            Name = Guid.NewGuid().ToString()
        };
        var createResponse = await client.PutAsJsonAsync("api/Cookbook/AddRecipe", createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var response = await client.GetAsync(createResponse.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}