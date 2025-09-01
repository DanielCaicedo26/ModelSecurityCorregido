using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Data.Interfaces;

namespace Data
{
    public class PermissionData
    {
        private readonly IDbContextProvider _dbContextProvider;
        private readonly ILogger<PermissionData> _logger;

        public PermissionData(IDbContextProvider dbContextProvider, ILogger<PermissionData> logger)
        {
            _dbContextProvider = dbContextProvider;
            _logger = logger;
        }

        private ApplicationDbContext _context => _dbContextProvider.GetDbContext();

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