using Data.Core;
using Data.Interfaces;
using Entity.Context;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Entity.Services;

namespace Data.Repositories
{
    /// <summary>
    /// Implementación del repositorio para la entidad Permission.
    /// </summary>
    public class PermissionRepository : GenericRepository<Permission>, IPermissionRepository
    {
        public PermissionRepository(IDynamicDbContextService dynamicContext, ILogger<PermissionRepository> logger)
            : base(dynamicContext, logger)
        {
        }
    }
}