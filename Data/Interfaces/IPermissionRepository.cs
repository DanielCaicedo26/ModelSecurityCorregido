using Data.Core;
using Entity.Model;

namespace Data.Interfaces
{
    /// <summary>
    /// Interfaz para el repositorio de permisos.
    /// </summary>
    public interface IPermissionRepository : IServiceBase<Permission>
    {
        // Sin métodos adicionales, solo hereda de IServiceBase<Permission>
    }
}