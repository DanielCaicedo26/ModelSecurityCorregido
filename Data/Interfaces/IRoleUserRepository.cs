using Data.Core;
using Entity.Model;

namespace Data.Interfaces
{
    /// <summary>
    /// Interfaz que define las operaciones del repositorio para la entidad RoleUser.
    /// </summary>
    public interface IRoleUserRepository : IServiceBase<RoleUser>
    {
        /// <summary>
        /// Obtiene asignaciones de roles por ID de usuario.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        /// <returns>Lista de asignaciones de roles para el usuario.</returns>
        Task<IEnumerable<RoleUser>> GetByUserIdAsync(int userId);

        /// <summary>
        /// Obtiene asignaciones de roles por ID de rol.
        /// </summary>
        /// <param name="roleId">ID del rol.</param>
        /// <returns>Lista de asignaciones de usuarios a ese rol.</returns>
        Task<IEnumerable<RoleUser>> GetByRoleIdAsync(int roleId);

        /// <summary>
        /// Verifica si un usuario tiene asignado un rol específico.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        /// <param name="roleId">ID del rol.</param>
        /// <returns>True si el usuario tiene el rol, False si no.</returns>
        Task<bool> HasRoleAsync(int userId, int roleId);
    }
}