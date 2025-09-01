using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Data.Interfaces;

namespace Data
{
    public class RoleUserData
    {
        private readonly IDbContextProvider _dbContextProvider;
        private readonly ILogger<RoleUserData> _logger;

        public RoleUserData(IDbContextProvider dbContextProvider, ILogger<RoleUserData> logger)
        {
            _dbContextProvider = dbContextProvider;
            _logger = logger;
        }

        private ApplicationDbContext _context => _dbContextProvider.GetDbContext();

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