using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Data.Interfaces;

namespace Data
{
    public class RoleFormPermissionData
    {
        private readonly IDbContextProvider _dbContextProvider;
        private readonly ILogger<RoleFormPermissionData> _logger;

        public RoleFormPermissionData(IDbContextProvider dbContextProvider, ILogger<RoleFormPermissionData> logger)
        {
            _dbContextProvider = dbContextProvider;
            _logger = logger;
        }

        private ApplicationDbContext _context => _dbContextProvider.GetDbContext();

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

        public async Task<bool> UpdateAsync(RoleFormPermission roleFormPermission)
        {
            try
            {
                var existingRoleFormPermission = await _context.Set<RoleFormPermission>().FindAsync(roleFormPermission.Id);
                if (existingRoleFormPermission == null)
                {
                    _logger.LogWarning("No se encontró el permiso de formulario de rol con ID {RoleFormPermissionId} para actualizar", roleFormPermission.Id);
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

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un permiso de formulario de rol con ID inválido: {RoleFormPermissionId}", id);
                return false;
            }

            try
            {
                var roleFormPermission = await _context.Set<RoleFormPermission>().FindAsync(id);
                if (roleFormPermission == null)
                {
                    _logger.LogInformation("No se encontró ningún permiso de formulario de rol con ID: {RoleFormPermissionId}", id);
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