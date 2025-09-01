using Data.Core;
using Data.Interfaces;
using Entity.Context;
using Entity.Model;
using Microsoft.Extensions.Logging;

namespace Data.Repositories
{
    public class PermissionRepository : GenericRepository<Permission>, IPermissionRepository
    {
        public PermissionRepository(IDbContextProvider dbContextProvider, ILogger<PermissionRepository> logger)
            : base(dbContextProvider, logger)
        {
        }
    }
}