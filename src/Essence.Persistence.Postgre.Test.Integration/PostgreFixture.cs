using Essence.Persistence.Postgre.Configuration;
using Essence.Persistence.Postgre.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;

namespace Essence.Persistence.Postgre.Test.Integration;

public class PostgreFixture : IDisposable
{
    private PostgreConnectionProvider? _connectionProvider;

    private IngredientRepository _ingredientRepository;
    private RecipeRepository _recipeRepository;

    public PostgreFixture()
    {
        var configurationRoot = new ConfigurationBuilder()
            .AddUserSecrets<PostgreFixture>()
            .Build();

        var options = configurationRoot
            .GetSection(PostgreConnectionOptions.PostgreConnection)
            .Get<PostgreConnectionOptions>()!;

        _connectionProvider = new PostgreConnectionProvider(
            Options.Create(options));

        _ingredientRepository = new IngredientRepository(_connectionProvider);
        _recipeRepository = new RecipeRepository(_connectionProvider);
    }

    internal IPostgreConnectionProvider ConnectionProvider => _connectionProvider!;

    internal IngredientRepository IngredientRepository => _ingredientRepository;

    internal RecipeRepository RecipeRepository => _recipeRepository;

    protected virtual void Dispose(bool disposing)
    {
        if (_connectionProvider is not null)
        {
            if (disposing)
            {
                _connectionProvider.Dispose();
            }
            _connectionProvider = null;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
