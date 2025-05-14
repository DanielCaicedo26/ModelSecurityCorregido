using Bussines.Core;
using Entity.Dto;
using Entity.Model;

namespace Bussines.interfaces
{
    /// <summary>
    /// Interfaz para el servicio de negocio de permisos.
    /// </summary>
    public interface IPermissionBusiness : IServiceBase<PermissionDto, Permission>
    {
        // Sin métodos adicionales, solo hereda de IServiceBase<PermissionDto, Permission>
    }
}