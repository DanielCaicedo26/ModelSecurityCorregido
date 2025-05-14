using Bussines.Core;
using Bussines.interfaces;
using Data.Interfaces;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;

namespace Bussines.Services
{
    /// <summary>
    /// Implementación del servicio de negocio para permisos.
    /// </summary>
    public class PermissionBusiness : ServiceBase<PermissionDto, Permission>, IPermissionBusiness
    {
        public PermissionBusiness(IPermissionRepository repository, ILogger<PermissionBusiness> logger)
            : base(repository, logger)
        {
        }
    }
}