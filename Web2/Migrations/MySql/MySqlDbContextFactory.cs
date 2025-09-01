using Entity.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Web2.Migrations.MySql
{
    public class MySqlDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContextMySql>
    {
        public ApplicationDbContextMySql CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContextMySql>();
            
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            optionsBuilder.UseMySql("Server=localhost;Port=3307;Database=modelsecurityda;User=mysqluser;Password=StrongPwd!123;", 
                ServerVersion.Parse("8.4.6-mysql"),
                b => b.MigrationsAssembly("Web2"));

            return new ApplicationDbContextMySql(optionsBuilder.Options, configuration);
        }
    }
}