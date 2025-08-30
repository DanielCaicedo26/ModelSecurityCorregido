using Entity.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Web2
{
    public class SqlServerContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Migration.json", optional: false)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TempDb;Trusted_Connection=true;TrustServerCertificate=true;", 
                b => b.MigrationsAssembly("Web2"));

            return new ApplicationDbContext(optionsBuilder.Options, configuration);
        }
    }

    public class PostgresContextFactory : IDesignTimeDbContextFactory<ApplicationDbContextPostgres>
    {
        public ApplicationDbContextPostgres CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Migration.json", optional: false)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContextPostgres>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=tempdb;Username=temp;Password=temp;",
                b => b.MigrationsAssembly("Web2"));

            return new ApplicationDbContextPostgres(optionsBuilder.Options, configuration);
        }
    }

    public class MySqlContextFactory : IDesignTimeDbContextFactory<ApplicationDbContextMySql>
    {
        public ApplicationDbContextMySql CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Migration.json", optional: false)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContextMySql>();
            optionsBuilder.UseMySql(
                "Server=localhost;Port=3306;Database=tempdb;User=temp;Password=temp;",
                ServerVersion.Parse("8.0.0"),
                b => b.MigrationsAssembly("Web2"));

            return new ApplicationDbContextMySql(optionsBuilder.Options, configuration);
        }
    }
}