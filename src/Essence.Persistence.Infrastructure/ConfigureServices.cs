using Essence.Domain.Repositories;
using Essence.Persistence.Postgre;
using Essence.Persistence.Postgre.Configuration;
using Essence.Persistence.Postgre.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Essence.Persistence.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {  
        services.Configure<PostgreConnectionOptions>(configuration.GetSection(PostgreConnectionOptions.PostgreConnection));

        services.AddSingleton<IPostgreConnectionProvider, PostgreConnectionProvider>();

        services.AddScoped<ICookbookRepository, CookbookRepository>();

        return services;
    }
}