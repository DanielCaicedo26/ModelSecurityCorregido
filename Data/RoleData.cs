using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Web2.Services;

namespace Data
{
    public class RoleData
    {
        private readonly IDbContextProvider _dbContextProvider;
        private readonly ILogger<RoleData> _logger;

        public RoleData(IDbContextProvider dbContextProvider, ILogger<RoleData> logger)
        {
            _dbContextProvider = dbContextProvider;
            _logger = logger;
        }

        private ApplicationDbContext _context => _dbContextProvider.GetDbContext();

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