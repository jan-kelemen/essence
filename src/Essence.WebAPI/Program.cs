using Essence.Domain.Infrastructure;
using Essence.Persistence.Postgre;
using Essence.Persistence.Postgre.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Essence.WebAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDomainServices();

        builder.Services.Configure<PostgreConnectionOptions>(
            builder.Configuration.GetSection(PostgreConnectionOptions.PostgreConnection));

        builder.Services.AddPersistenceServices();

        builder.Services.AddControllers();

        var app = builder.Build();

        app.MapControllers();

        app.Run();
    }
}