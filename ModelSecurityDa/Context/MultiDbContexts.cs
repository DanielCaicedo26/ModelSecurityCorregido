using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Entity.Context
{
    public class ApplicationDbContextPostgres : ApplicationDbContext
    {
        public ApplicationDbContextPostgres(DbContextOptions<ApplicationDbContextPostgres> options, IConfiguration configuration)
            : base(options, configuration) { }
    }

    public class ApplicationDbContextMySql : ApplicationDbContext
    {
        public ApplicationDbContextMySql(DbContextOptions<ApplicationDbContextMySql> options, IConfiguration configuration)
            : base(options, configuration) { }
    }
}
