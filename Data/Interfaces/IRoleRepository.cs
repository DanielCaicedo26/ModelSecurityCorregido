using Data.Core;
using Entity.Model;

namespace Data.Interfaces
{
    /// <summary>
    /// Interfaz que define las operaciones del repositorio para la entidad Role.
    /// </summary>
    public interface IRoleRepository : IServiceBase<Role>
    {
        /// <summary>
        /// Obtiene un rol por su nombre.
        /// </summary>
        /// <param name="roleName">Nombre del rol a buscar.</param>
        /// <returns>El rol encontrado o null si no existe.</returns>
        Task<Role?> GetByNameAsync(string roleName);

        /// <summary>
        /// Obtiene todos los roles asociados a un usuario específico.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        /// <returns>Lista de roles asociados al usuario.</returns>
        Task<IEnumerable<Role>> GetByUserIdAsync(int userId);
    }
}