using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gestión de la entidad Permission en la base de datos.
    /// </summary>
    public class PermissionData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PermissionData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{PermissionData}"/> para el registro de logs.</param>
        public PermissionData(ApplicationDbContext context, ILogger<PermissionData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los permisos almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de permisos.</returns>
        public async Task<IEnumerable<Permission>> GetAllAsync()
        {
            try
            {
                return await _context.Set<Permission>()
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los permisos");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un permiso específico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del permiso.</param>
        /// <returns>El permiso encontrado o null si no existe.</returns>
        public async Task<Permission?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Permission>()
                    .FirstOrDefaultAsync(p => p.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el permiso con ID {PermissionId}", id);
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo permiso en la base de datos.
        /// </summary>
        /// <param name="permission">Instancia del permiso a crear.</param>
        /// <returns>El permiso creado.</returns>
        public async Task<Permission> CreateAsync(Permission permission)
        {
            try
            {
                await _context.Set<Permission>().AddAsync(permission);
                await _context.SaveChangesAsync();
                return permission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el permiso");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un permiso existente en la base de datos.
        /// </summary>
        /// <param name="permission">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Permission permission)
        {
            try
            {
                var existingPermission = await _context.Set<Permission>().FindAsync(permission.Id);
                if (existingPermission == null)
                {
                    _logger.LogWarning("No se encontró el permiso con ID {PermissionId} para actualizar", permission.Id);
                    return false;
                }

                _context.Entry(existingPermission).CurrentValues.SetValues(permission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el permiso");
                return false;
            }
        }

        /// <summary>
        /// Elimina un permiso de la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del permiso a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un permiso con ID inválido: {PermissionId}", id);
                return false;
            }

            try
            {
                var permission = await _context.Set<Permission>().FindAsync(id);
                if (permission == null)
                {
                    _logger.LogInformation("No se encontró ningún permiso con ID: {PermissionId}", id);
                    return false;
                }

                _context.Set<Permission>().Remove(permission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el permiso con ID {PermissionId}", id);
                return false;
            }
        }
    }
}

