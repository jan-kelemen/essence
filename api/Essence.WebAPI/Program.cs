using Essence.Domain.Infrastructure;
using Essence.Persistence.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Essence.WebAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var apiCorsPolicy = "ApiCorsPolicy";
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(
                name: apiCorsPolicy,
                builder =>
                {
                    builder.WithOrigins("http://localhost:4200", "https://localhost:4200")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
                });
        });

        builder.Services.AddPersistenceServices(builder.Configuration);

        builder.Services.AddDomainServices();

        builder.Services.AddControllers();
        
        var app = builder.Build();
        app.UseHttpsRedirection();
        app.UseCors(apiCorsPolicy);

        app.MapControllers();

        app.Run();
    }
}
