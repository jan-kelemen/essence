using Essence.Domain.Services;
using Essence.Domain.Services.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Essence.Domain.Infrastructure;

public static class ConfigureServices
{
  public static IServiceCollection AddDomainServices(this IServiceCollection services)
  {
    services.AddScoped<ICookbookService, CookbookService>();

    return services;
  }
}