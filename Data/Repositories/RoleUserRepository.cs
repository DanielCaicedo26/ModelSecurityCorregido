using Data.Core;
using Data.Interfaces;
using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repositories
{
    /// <summary>
    /// Implementación del repositorio para la entidad RoleUser.
    /// </summary>
    public class RoleUserRepository : GenericRepository<RoleUser>, IRoleUserRepository
    {
        public RoleUserRepository(ApplicationDbContext context, ILogger<RoleUserRepository> logger)
            : base(context, logger)
        {
        }

        /// <summary>
        /// Obtiene todas las asignaciones de roles con sus relaciones.
        /// </summary>
        public override async Task<IEnumerable<RoleUser>> GetAllAsync()
        {
            return await _context.RoleUser
                .Include(ru => ru.User)
                .Include(ru => ru.Role)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene una asignación de rol por su ID con sus relaciones.
        /// </summary>
        public override async Task<RoleUser?> GetByIdAsync(int id)
        {
            return await _context.RoleUser
                .Include(ru => ru.User)
                .Include(ru => ru.Role)
                .FirstOrDefaultAsync(ru => ru.Id == id);
        }

        /// <summary>
        /// Obtiene asignaciones de roles por ID de usuario.
        /// </summary>
        public async Task<IEnumerable<RoleUser>> GetByUserIdAsync(int userId)
        {
            return await _context.RoleUser
                .Include(ru => ru.Role)
                .Where(ru => ru.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene asignaciones de roles por ID de rol.
        /// </summary>
        public async Task<IEnumerable<RoleUser>> GetByRoleIdAsync(int roleId)
        {
            return await _context.RoleUser
                .Include(ru => ru.User)
                    .ThenInclude(u => u.Person)
                .Where(ru => ru.RoleId == roleId)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Actualiza una asignación de rol existente.
        /// </summary>
        public override async Task<bool> UpdateAsync(RoleUser roleUser)
        {
            var existingRoleUser = await _context.RoleUser.FindAsync(roleUser.Id);
            if (existingRoleUser == null)
            {
                _logger.LogWarning("No se encontró la asignación de rol con ID {RoleUserId} para actualizar", roleUser.Id);
                return false;
            }

            // Preservar la fecha de creación original
            roleUser.CreatedAt = existingRoleUser.CreatedAt;

            _context.Entry(existingRoleUser).CurrentValues.SetValues(roleUser);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Elimina una asignación de rol físicamente.
        /// </summary>
        public override async Task<bool> DeleteAsync(int id)
        {
            var roleUser = await _context.RoleUser.FindAsync(id);
            if (roleUser == null)
                return false;

            _context.RoleUser.Remove(roleUser);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Realiza una eliminación lógica de la asignación de rol.
        /// </summary>
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