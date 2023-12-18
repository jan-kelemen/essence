using Essence.Domain.Repositories;
using Essence.Persistence.Postgre;
using Essence.Persistence.Postgre.Configuration;
using Essence.Persistence.Postgre.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Essence.Persistence.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {  
        services.Configure<PostgreConnectionOptions>(configuration.GetSection(PostgreConnectionOptions.PostgreConnection));

        services.AddOpenTelemetry().WithTracing(tracing => tracing.AddNpgsql());

        services.AddSingleton<IPostgreConnectionProvider, PostgreConnectionProvider>();

        services.AddScoped<IIngredientRepository, IngredientRepository>();

        services.AddScoped<IRecipeRepository, RecipeRepository>();

        return services;
    }
}
