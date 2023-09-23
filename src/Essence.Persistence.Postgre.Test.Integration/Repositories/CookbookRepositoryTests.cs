using Essence.Domain.Services;
using Essence.Domain.Vocabulary;
using Essence.Persistence.Postgre.Repositories;
using System;
using Xunit;

namespace Essence.Persistence.Postgre.Test.Integration.Repositories;

[Collection(nameof(PostgreCollection))]
public class CookbookRepositoryTests
{
    private readonly PostgreConnectionProvider _connectionProvider;

    public CookbookRepositoryTests(PostgreFixture postgreFixture)
    {
        _connectionProvider = postgreFixture.ConnectionProvider;
    }

    [Fact]
    public async void GetRecipe_ReturnsNotFound_GivenNonexistingIdentifier()
    {
        var cookbookRepository = new CookbookRepository(_connectionProvider);

        var searchedIdentifier = new Identifier(Guid.NewGuid().ToString());

        var recipe = await cookbookRepository.GetRecipe(searchedIdentifier);

        var notFoundError = Assert.IsType<GetRecipeError.NotFound>(recipe.ExpectError());
        Assert.Equal(searchedIdentifier, notFoundError.Id);
    }
}
