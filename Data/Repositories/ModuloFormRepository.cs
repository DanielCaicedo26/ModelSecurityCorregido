using Data.Core;
using Data.Interfaces;
using Entity.Context;
using Entity.Model;
using Microsoft.Extensions.Logging;

namespace Data.Repositories
{
    public class ModuloFormRepository : GenericRepository<ModuloForm>, IModuloFormRepository
    {
        public ModuloFormRepository(ApplicationDbContext context, ILogger<ModuloFormRepository> logger)
            : base(context, logger)
        {
        }
    }
}