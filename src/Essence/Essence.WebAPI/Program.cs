using Essence.Domain.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Essence.WebAPI;

public class Program
{
  public static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddDomainServices();

    builder.Services.AddControllers();

    var app = builder.Build();

    app.MapControllers();

    app.Run();
  }
}