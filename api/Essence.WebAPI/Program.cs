using Essence.Domain.Infrastructure;
using Essence.Persistence.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Extensions.Hosting;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using System;

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

        var tracingOtlpEndpoint = builder.Configuration["OTLP_ENDPOINT_URL"];
        var otel = builder.Services.AddOpenTelemetry();
        
        otel.ConfigureResource(resource => resource
            .AddService(serviceName: builder.Environment.ApplicationName));
        
        otel.WithTracing(tracing =>
        {
            tracing.AddAspNetCoreInstrumentation();
            tracing.AddHttpClientInstrumentation();
            if (tracingOtlpEndpoint != null)
            {
                tracing.AddOtlpExporter(otlpOptions =>
                 {
                     otlpOptions.Endpoint = new Uri(tracingOtlpEndpoint);
                 });
            }
            else
            {
                tracing.AddConsoleExporter();
            }
        });

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
