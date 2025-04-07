using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gestión de la entidad Role en la base de datos.
    /// </summary>
    public class RoleData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RoleData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{RoleData}"/> para el registro de logs.</param>
        public RoleData(ApplicationDbContext context, ILogger<RoleData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los roles almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de roles.</returns>
        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            try
            {
                return await _context.Set<Role>()
                    .Include(r => r.RoleUsers)
                    .Include(r => r.RoleFormPermissions)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los roles");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un rol específico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del rol.</param>
        /// <returns>El rol encontrado o null si no existe.</returns>
        public async Task<Role?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Role>()
                    .Include(r => r.RoleUsers)
                    .Include(r => r.RoleFormPermissions)
                    .FirstOrDefaultAsync(r => r.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol con ID {RoleId}", id);
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo rol en la base de datos.
        /// </summary>
        /// <param name="role">Instancia del rol a crear.</param>
        /// <returns>El rol creado.</returns>
        public async Task<Role> CreateAsync(Role role)
        {
            try
            {
                await _context.Set<Role>().AddAsync(role);
                await _context.SaveChangesAsync();
                return role;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el rol");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un rol existente en la base de datos.
        /// </summary>
        /// <param name="role">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Role role)
        {
            try
            {
                var existingRole = await _context.Set<Role>().FindAsync(role.Id);
                if (existingRole == null)
                {
                    _logger.LogWarning("No se encontró el rol con ID {RoleId} para actualizar", role.Id);
                    return false;
                }

                _context.Entry(existingRole).CurrentValues.SetValues(role);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el rol");
                return false;
            }
        }

        /// <summary>
        /// Elimina un rol de la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del rol a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un rol con ID inválido: {RoleId}", id);
                return false;
            }

            try
            {
                var role = await _context.Set<Role>().FindAsync(id);
                if (role == null)
                {
                    _logger.LogInformation("No se encontró ningún rol con ID: {RoleId}", id);
                    return false;
                }

                _context.Set<Role>().Remove(role);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el rol con ID {RoleId}", id);
                return false;
            }
        }
    }
}


