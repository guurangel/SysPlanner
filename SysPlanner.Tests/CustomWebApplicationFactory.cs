using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using SysPlanner.Infrastructure.Contexts;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove o DbContext real
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<SysPlannerDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Adiciona DbContext InMemory
            services.AddDbContext<SysPlannerDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            // Build provider
            var sp = services.BuildServiceProvider();

            // Inicializar banco
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SysPlannerDbContext>();
            db.Database.EnsureCreated();
        });
    }
}
