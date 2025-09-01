using Entity.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Web2.Migrations.Postgres
{
    public class PostgresDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContextPostgres>
    {
        public ApplicationDbContextPostgres CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContextPostgres>();
            
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=modelsecurityda;Username=postgres;Password=StrongPwd!123;",
                b => b.MigrationsAssembly("Web2"));

            return new ApplicationDbContextPostgres(optionsBuilder.Options, configuration);
        }
    }
}