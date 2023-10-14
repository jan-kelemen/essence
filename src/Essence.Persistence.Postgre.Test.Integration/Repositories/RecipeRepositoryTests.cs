using Essence.Domain.Services;
using Essence.Domain.Vocabulary;
using Essence.Persistence.Postgre.Repositories;
using System;
using Xunit;

namespace Essence.Persistence.Postgre.Test.Integration.Repositories;

[Collection(nameof(PostgreCollection))]
public class RecipeRepositoryTests
{
    private readonly IPostgreConnectionProvider _connectionProvider;

    public RecipeRepositoryTests(PostgreFixture postgreFixture)
    {
        _connectionProvider = postgreFixture.ConnectionProvider;
    }

    [Fact]
    public async void GetRecipe_ReturnsNotFound_GivenNonexistingIdentifier()
    {
        var recipeRepository = new RecipeRepository(_connectionProvider);

        var searchedIdentifier = new Identifier(Guid.NewGuid().ToString());

        var recipe = await recipeRepository.GetRecipe(searchedIdentifier);

        var notFoundError = Assert.IsType<GetRecipeError.NotFound>(recipe.ExpectError());
        Assert.Equal(searchedIdentifier, notFoundError.Id);
    }
}
