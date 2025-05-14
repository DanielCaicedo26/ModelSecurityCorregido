using Bussines.Core;
using Entity.Dto;
using Entity.Model;

namespace Bussines.interfaces
{
    public interface IRoleBusiness : IServiceBase<RoleDto, Role>
    {
        /// <summary>
        /// Obtiene un rol por su nombre.
        /// </summary>
        /// <param name="roleName">Nombre del rol a buscar.</param>
        /// <returns>El RoleDto encontrado o null si no existe.</returns>
        Task<RoleDto?> GetByNameAsync(string roleName);

        /// <summary>
        /// Obtiene todos los roles asociados a un usuario específico.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        /// <returns>Lista de roles asociados al usuario.</returns>
        Task<IEnumerable<RoleDto>> GetByUserIdAsync(int userId);

        /// <summary>
        /// Actualiza el nombre y la descripción de un rol.
        /// </summary>
        /// <param name="id">ID del rol a actualizar.</param>
        /// <param name="roleName">Nuevo nombre del rol.</param>
        /// <param name="description">Nueva descripción del rol.</param>
        /// <returns>El RoleDto actualizado.</returns>
        Task<RoleDto> Update(int id, string roleName, string? description);
    }
}