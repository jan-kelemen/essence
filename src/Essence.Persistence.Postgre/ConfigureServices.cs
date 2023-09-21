using Essence.Domain.Repositories;
using Essence.Persistence.Postgre.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Essence.Persistence.Postgre;

public static class ConfigureServices
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
    {
        services.AddSingleton<IPostgreConnectionProvider, PostgreConnectionProvider>();

        services.AddScoped<ICookbookRepository, CookbookRepository>();

        return services;
    }
}