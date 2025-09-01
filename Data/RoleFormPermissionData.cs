using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entity.Services;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gesti�n de la entidad RoleFormPermission en la base de datos.
    /// </summary>
    public class RoleFormPermissionData
    {
        private readonly IDynamicDbContextService _dynamicContext;
        private ApplicationDbContext _context => _dynamicContext.GetCurrentApplicationContext();
        private readonly ILogger<RoleFormPermissionData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexi�n con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{RoleFormPermissionData}"/> para el registro de logs.</param>
        public RoleFormPermissionData(IDynamicDbContextService dynamicContext, ILogger<RoleFormPermissionData> logger)
        {
            _dynamicContext = dynamicContext;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los permisos de formulario de rol almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de permisos de formulario de rol.</returns>
        public async Task<IEnumerable<RoleFormPermission>> GetAllAsync()
        {
            try
            {
                return await _context.Set<RoleFormPermission>()
                    .Include(rfp => rfp.Role)
                    .Include(rfp => rfp.Form)
                    .Include(rfp => rfp.Permission)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los permisos de formulario de rol");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un permiso de formulario de rol espec�fico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del permiso de formulario de rol.</param>
        /// <returns>El permiso de formulario de rol encontrado o null si no existe.</returns>
        public async Task<RoleFormPermission?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<RoleFormPermission>()
                    .Include(rfp => rfp.Role)
                    .Include(rfp => rfp.Form)
                    .Include(rfp => rfp.Permission)
                    .FirstOrDefaultAsync(rfp => rfp.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el permiso de formulario de rol con ID {RoleFormPermissionId}", id);
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo permiso de formulario de rol en la base de datos.
        /// </summary>
        /// <param name="roleFormPermission">Instancia del permiso de formulario de rol a crear.</param>
        /// <returns>El permiso de formulario de rol creado.</returns>
        public async Task<RoleFormPermission> CreateAsync(RoleFormPermission roleFormPermission)
        {
            try
            {
                await _context.Set<RoleFormPermission>().AddAsync(roleFormPermission);
                await _context.SaveChangesAsync();
                return roleFormPermission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el permiso de formulario de rol");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un permiso de formulario de rol existente en la base de datos.
        /// </summary>
        /// <param name="roleFormPermission">Objeto con la informaci�n actualizada.</param>
        /// <returns>True si la operaci�n fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(RoleFormPermission roleFormPermission)
        {
            try
            {
                var existingRoleFormPermission = await _context.Set<RoleFormPermission>().FindAsync(roleFormPermission.Id);
                if (existingRoleFormPermission == null)
                {
                    _logger.LogWarning("No se encontr� el permiso de formulario de rol con ID {RoleFormPermissionId} para actualizar", roleFormPermission.Id);
                    return false;
                }

                _context.Entry(existingRoleFormPermission).CurrentValues.SetValues(roleFormPermission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el permiso de formulario de rol");
                return false;
            }
        }

        /// <summary>
        /// Elimina un permiso de formulario de rol de la base de datos.
        /// </summary>
        /// <param name="id">Identificador �nico del permiso de formulario de rol a eliminar.</param>
        /// <returns>True si la eliminaci�n fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� eliminar un permiso de formulario de rol con ID inv�lido: {RoleFormPermissionId}", id);
                return false;
            }

            try
            {
                var roleFormPermission = await _context.Set<RoleFormPermission>().FindAsync(id);
                if (roleFormPermission == null)
                {
                    _logger.LogInformation("No se encontr� ning�n permiso de formulario de rol con ID: {RoleFormPermissionId}", id);
                    return false;
                }

                _context.Set<RoleFormPermission>().Remove(roleFormPermission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el permiso de formulario de rol con ID {RoleFormPermissionId}", id);
                return false;
            }
        }
    }
}



