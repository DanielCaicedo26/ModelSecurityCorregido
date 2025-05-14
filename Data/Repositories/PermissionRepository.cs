using Data.Core;
using Data.Interfaces;
using Entity.Context;
using Entity.Model;
using Microsoft.Extensions.Logging;

namespace Data.Repositories
{
    /// <summary>
    /// Implementación del repositorio para la entidad Permission.
    /// </summary>
    public class PermissionRepository : GenericRepository<Permission>, IPermissionRepository
    {
        public PermissionRepository(ApplicationDbContext context, ILogger<PermissionRepository> logger)
            : base(context, logger)
        {
        }
    }
}