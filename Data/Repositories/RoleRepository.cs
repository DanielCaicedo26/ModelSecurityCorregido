using Data.Core;
using Data.Interfaces;
using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(IDbContextProvider dbContextProvider, ILogger<RoleRepository> logger)
            : base(dbContextProvider, logger)
        {
        }

        public override async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _context.Role
                .Include(r => r.RoleUsers)
                .Include(r => r.RoleFormPermissions)
                .AsNoTracking()
                .ToListAsync();
        }

        public override async Task<Role?> GetByIdAsync(int id)
        {
            return await _context.Role
                .Include(r => r.RoleUsers)
                .Include(r => r.RoleFormPermissions)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Role?> GetByNameAsync(string roleName)
        {
            return await _context.Role
                .Include(r => r.RoleUsers)
                .Include(r => r.RoleFormPermissions)
                .FirstOrDefaultAsync(r => r.RoleName.ToLower() == roleName.ToLower());
        }

        public async Task<IEnumerable<Role>> GetByUserIdAsync(int userId)
        {
            return await _context.RoleUser
                .Where(ru => ru.UserId == userId && ru.IsActive)
                .Include(ru => ru.Role)
                .Select(ru => ru.Role)
                .Where(r => r.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public override async Task<Role> AddAsync(Role role)
        {
            role.CreatedAt = DateTime.UtcNow;
            role.IsActive = true;
            await _context.Role.AddAsync(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public override async Task<bool> UpdateAsync(Role role)
        {
            var existingRole = await _context.Role.FindAsync(role.Id);
            if (existingRole == null)
                return false;

            role.CreatedAt = existingRole.CreatedAt;

            _context.Entry(existingRole).CurrentValues.SetValues(role);
            await _context.SaveChangesAsync();
            return true;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var role = await _context.Role.FindAsync(id);
            if (role == null)
                return false;

            _context.Role.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }

        public override async Task<bool> DeleteLogicalAsync(int id)
        {
            var role = await _context.Role.FindAsync(id);
            if (role == null)
                return false;

            role.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}