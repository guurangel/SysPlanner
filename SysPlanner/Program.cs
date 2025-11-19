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
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Instrumentation.Http;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ---------------------------------------------------------
        // CONFIGURAÇÕES
        // ---------------------------------------------------------

        // Carregar variáveis de ambiente
        builder.Configuration.AddEnvironmentVariables();

        // Conexão Oracle (com fallback para env variable)
        var oracleConnectionString = builder.Configuration.GetConnectionString("Oracle")
            ?? Environment.GetEnvironmentVariable("ORACLE_CONNECTION_STRING");

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
        // CORS – LIBERAR ACESSO AO FRONT-END
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
        // VERSIONAMENTO
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
        // INJEÇÃO DE DEPENDÊNCIA
        // ---------------------------------------------------------

        builder.Services.AddScoped<IUsuarioService, UsuarioService>();
        builder.Services.AddScoped<ILembreteService, LembreteService>();

        // ---------------------------------------------------------
        // TRATAMENTO GLOBAL DE ERROS (IMPORTANTE!)
        // ---------------------------------------------------------

        builder.Services.AddProblemDetails();

        // Logging já é configurado automaticamente, mas podemos adicionar níveis
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();
        builder.Logging.SetMinimumLevel(LogLevel.Information);

        // OpenTelemetry Tracing
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

        // Controllers
        app.MapControllers();

        // Health Check
        app.MapHealthChecks("/health");

        app.Run();
    }
}
