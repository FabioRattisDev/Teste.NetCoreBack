using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FullstackApp.Infrastructure
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)                
                .Build();

            var conn =
                configuration.GetConnectionString("Default") ?? // pega do appsettings
                Environment.GetEnvironmentVariable("ConnectionStrings__Default") ?? // .env/dock
                Environment.GetEnvironmentVariable("DATABASE_CONNECTION") ?? // fallback
                "Host=localhost;Port=5432;Database=fullstackdb;Username=dev;Password=dev123"; // último recurso

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(conn);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
