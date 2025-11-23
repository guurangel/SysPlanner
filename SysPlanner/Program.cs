using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json.Serialization;
using SysPlanner.Infrastructure.Contexts;
using SysPlanner.Services.Interfaces;
using SysPlanner.Services;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ---------------------------------------------------------
        // PORTA (necessária para Render)
        // ---------------------------------------------------------
        var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
        builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

        // ---------------------------------------------------------
        // CONNECTION STRING (somente via Environment Variable)
        // ---------------------------------------------------------
        var oracleConnectionString = Environment.GetEnvironmentVariable("ORACLE_CONNECTION_STRING");

        if (string.IsNullOrEmpty(oracleConnectionString))
        {
            throw new Exception("A variável de ambiente ORACLE_CONNECTION_STRING não foi definida.");
        }

        builder.Services.AddDbContext<SysPlannerDbContext>(options =>
            options.UseOracle(oracleConnectionString));

        // ---------------------------------------------------------
        // HEALTH CHECK
        // ---------------------------------------------------------
        builder.Services.AddHealthChecks()
            .AddCheck("Database", () =>
            {
                try
                {
                    using var scope = builder.Services.BuildServiceProvider().CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<SysPlannerDbContext>();
                    db.Database.CanConnect();
                    return HealthCheckResult.Healthy("Conectado ao banco de dados com sucesso.");
                }
                catch (Exception ex)
                {
                    return HealthCheckResult.Unhealthy("Falha ao conectar ao banco.", ex);
                }
            });

        // ---------------------------------------------------------
        // CORS
        // ---------------------------------------------------------
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                policy => policy.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader());
        });

        // ---------------------------------------------------------
        // CONTROLLERS + JSON
        // ---------------------------------------------------------
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        // ---------------------------------------------------------
        // API VERSIONING
        // ---------------------------------------------------------
        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        });

        // ---------------------------------------------------------
        // SWAGGER
        // ---------------------------------------------------------
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // ---------------------------------------------------------
        // INJEÇÃO DE DEPENDÊNCIAS
        // ---------------------------------------------------------
        builder.Services.AddScoped<IUsuarioService, UsuarioService>();
        builder.Services.AddScoped<ILembreteService, LembreteService>();

        // ---------------------------------------------------------
        // GLOBAL ERROR HANDLING
        // ---------------------------------------------------------
        builder.Services.AddProblemDetails();

        // ---------------------------------------------------------
        // LOGGING
        // ---------------------------------------------------------
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();
        builder.Logging.SetMinimumLevel(LogLevel.Information);

        // ---------------------------------------------------------
        // OPENTELEMETRY (LOGS / TRACING / SPANS)
        // ---------------------------------------------------------
        builder.Services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("SysPlanner"))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddConsoleExporter();
            });

        var app = builder.Build();

        // ---------------------------------------------------------
        // PIPELINE
        // ---------------------------------------------------------

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();

        // CORS
        app.UseCors("AllowAll");

        app.MapControllers();
        app.MapHealthChecks("/health");

        app.Run();
    }
}
