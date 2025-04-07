using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gestión de la entidad RoleUser en la base de datos.
    /// </summary>
    public class RoleUserData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RoleUserData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{RoleUserData}"/> para el registro de logs.</param>
        public RoleUserData(ApplicationDbContext context, ILogger<RoleUserData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los usuarios de rol almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de usuarios de rol.</returns>
        public async Task<IEnumerable<RoleUser>> GetAllAsync()
        {
            try
            {
                return await _context.Set<RoleUser>()
                    .Include(ru => ru.Role)
                    .Include(ru => ru.User)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios de rol");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un usuario de rol específico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del usuario de rol.</param>
        /// <returns>El usuario de rol encontrado o null si no existe.</returns>
        public async Task<RoleUser?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<RoleUser>()
                    .Include(ru => ru.Role)
                    .Include(ru => ru.User)
                    .FirstOrDefaultAsync(ru => ru.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario de rol con ID {RoleUserId}", id);
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo usuario de rol en la base de datos.
        /// </summary>
        /// <param name="roleUser">Instancia del usuario de rol a crear.</param>
        /// <returns>El usuario de rol creado.</returns>
        public async Task<RoleUser> CreateAsync(RoleUser roleUser)
        {
            try
            {
                await _context.Set<RoleUser>().AddAsync(roleUser);
                await _context.SaveChangesAsync();
                return roleUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el usuario de rol");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un usuario de rol existente en la base de datos.
        /// </summary>
        /// <param name="roleUser">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(RoleUser roleUser)
        {
            try
            {
                var existingRoleUser = await _context.Set<RoleUser>().FindAsync(roleUser.Id);
                if (existingRoleUser == null)
                {
                    _logger.LogWarning("No se encontró el usuario de rol con ID {RoleUserId} para actualizar", roleUser.Id);
                    return false;
                }

                _context.Entry(existingRoleUser).CurrentValues.SetValues(roleUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario de rol");
                return false;
            }
        }

        /// <summary>
        /// Elimina un usuario de rol de la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del usuario de rol a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un usuario de rol con ID inválido: {RoleUserId}", id);
                return false;
            }

            try
            {
                var roleUser = await _context.Set<RoleUser>().FindAsync(id);
                if (roleUser == null)
                {
                    _logger.LogInformation("No se encontró ningún usuario de rol con ID: {RoleUserId}", id);
                    return false;
                }

                _context.Set<RoleUser>().Remove(roleUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el usuario de rol con ID {RoleUserId}", id);
                return false;
            }
        }
    }
}



