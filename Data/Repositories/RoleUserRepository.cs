using Data.Core;
using Data.Interfaces;
using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repositories
{
    public class RoleUserRepository : GenericRepository<RoleUser>, IRoleUserRepository
    {
        public RoleUserRepository(IDbContextProvider dbContextProvider, ILogger<RoleUserRepository> logger)
            : base(dbContextProvider, logger)
        {
        }

        public override async Task<IEnumerable<RoleUser>> GetAllAsync()
        {
            return await _context.RoleUser
                .Include(ru => ru.User)
                .Include(ru => ru.Role)
                .AsNoTracking()
                .ToListAsync();
        }

        public override async Task<RoleUser?> GetByIdAsync(int id)
        {
            return await _context.RoleUser
                .Include(ru => ru.User)
                .Include(ru => ru.Role)
                .FirstOrDefaultAsync(ru => ru.Id == id);
        }

        public async Task<IEnumerable<RoleUser>> GetByUserIdAsync(int userId)
        {
            return await _context.RoleUser
                .Include(ru => ru.Role)
                .Where(ru => ru.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<RoleUser>> GetByRoleIdAsync(int roleId)
        {
            return await _context.RoleUser
                .Include(ru => ru.User)
                    .ThenInclude(u => u.Person)
                .Where(ru => ru.RoleId == roleId)
                .AsNoTracking()
                .ToListAsync();
        }

        public override async Task<bool> UpdateAsync(RoleUser roleUser)
        {
            var existingRoleUser = await _context.RoleUser.FindAsync(roleUser.Id);
            if (existingRoleUser == null)
            {
                _logger.LogWarning("No se encontró la asignación de rol con ID {RoleUserId} para actualizar", roleUser.Id);
                return false;
            }

            roleUser.CreatedAt = existingRoleUser.CreatedAt;

            _context.Entry(existingRoleUser).CurrentValues.SetValues(roleUser);
            await _context.SaveChangesAsync();
            return true;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var roleUser = await _context.RoleUser.FindAsync(id);
            if (roleUser == null)
                return false;

            _context.RoleUser.Remove(roleUser);
            await _context.SaveChangesAsync();
            return true;
        }

        public override async Task<bool> DeleteLogicalAsync(int id)
        {
            var roleUser = await _context.RoleUser.FindAsync(id);
            if (roleUser == null)
                return false;

            roleUser.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}