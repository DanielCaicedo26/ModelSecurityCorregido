using Data.Core;
using Data.Interfaces;
using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repositories
{
    /// <summary>
    /// Implementación del repositorio para la entidad Role.
    /// </summary>
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(ApplicationDbContext context, ILogger<RoleRepository> logger)
            : base(context, logger)
        {
        }

        /// <summary>
        /// Obtiene todos los roles con sus relaciones.
        /// </summary>
        public override async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _context.Role
                .Include(r => r.RoleUsers)
                .Include(r => r.RoleFormPermissions)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene un rol por su ID con sus relaciones.
        /// </summary>
        public override async Task<Role?> GetByIdAsync(int id)
        {
            return await _context.Role
                .Include(r => r.RoleUsers)
                .Include(r => r.RoleFormPermissions)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        /// <summary>
        /// Obtiene un rol por su nombre.
        /// </summary>
        public async Task<Role?> GetByNameAsync(string roleName)
        {
            return await _context.Role
                .Include(r => r.RoleUsers)
                .Include(r => r.RoleFormPermissions)
                .FirstOrDefaultAsync(r => r.RoleName.ToLower() == roleName.ToLower());
        }

        /// <summary>
        /// Obtiene todos los roles asociados a un usuario específico.
        /// </summary>
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

        /// <summary>
        /// Agrega un nuevo rol.
        /// </summary>
        public override async Task<Role> AddAsync(Role role)
        {
            role.CreatedAt = DateTime.UtcNow;
            role.IsActive = true;
            await _context.Role.AddAsync(role);
            await _context.SaveChangesAsync();
            return role;
        }

        /// <summary>
        /// Actualiza un rol existente.
        /// </summary>
        public override async Task<bool> UpdateAsync(Role role)
        {
            var existingRole = await _context.Role.FindAsync(role.Id);
            if (existingRole == null)
                return false;

            // Preservar la fecha de creación original
            role.CreatedAt = existingRole.CreatedAt;

            _context.Entry(existingRole).CurrentValues.SetValues(role);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Elimina un rol físicamente.
        /// </summary>
        public override async Task<bool> DeleteAsync(int id)
        {
            var role = await _context.Role.FindAsync(id);
            if (role == null)
                return false;

            _context.Role.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Realiza una eliminación lógica del rol.
        /// </summary>
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