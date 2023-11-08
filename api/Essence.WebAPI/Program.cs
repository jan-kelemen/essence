using Essence.Domain.Infrastructure;
using Essence.Persistence.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        builder.Services.AddPersistenceServices(builder.Configuration);

        builder.Services.AddDomainServices();

        builder.Services.AddControllers();
        
        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }
        app.UseHttpsRedirection();
        app.UseCors(apiCorsPolicy);

        app.MapControllers();

        app.Run();
    }
}
