using Data.Core;
using Data.Interfaces;
using Entity.Context;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Entity.Services;

namespace Data.Repositories
{
    public class ModuloFormRepository : GenericRepository<ModuloForm>, IModuloFormRepository
    {
        public ModuloFormRepository(IDynamicDbContextService dynamicContext, ILogger<ModuloFormRepository> logger)
            : base(dynamicContext, logger)
        {
        }
    }
}