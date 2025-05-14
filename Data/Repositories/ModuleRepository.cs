using Data.Core;
using Data.Interfaces;
using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repositories
{
    public class ModuleRepository : GenericRepository<Module>, IModuleRepository
    {
        public ModuleRepository(ApplicationDbContext context, ILogger<ModuleRepository> logger)
            : base(context, logger)
        {
        }

        public override async Task<IEnumerable<Module>> GetAllAsync()
        {
            return await _context.Module
                .Include(m => m.ModuloForms)
                .AsNoTracking()
                .ToListAsync();
        }

        public override async Task<Module?> GetByIdAsync(int id)
        {
            return await _context.Module
                .Include(m => m.ModuloForms)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Module>> GetByUserIdAsync(int userId)
        {
            // Obtener los módulos asociados a un usuario
            // Esta implementación presupone que existen relaciones definidas
            // Para asociar módulos con usuarios a través de formularios o roles
            var userRoleIds = await _context.RoleUser
                .Where(ru => ru.UserId == userId && ru.IsActive)
                .Select(ru => ru.RoleId)
                .ToListAsync();

            var formIds = await _context.RoleFormPermission
                .Where(rfp => userRoleIds.Contains(rfp.RoleId) && rfp.IsActive)
                .Select(rfp => rfp.FormId)
                .Distinct()
                .ToListAsync();

            return await _context.ModuloForm
                .Where(mf => formIds.Contains(mf.FormId) && mf.IsActive)
                .Select(mf => mf.Module)
                .Where(m => m != null && m.IsActive)
                .Distinct()
                .Include(m => m.ModuloForms)
                .AsNoTracking()
                .ToListAsync();
        }

        public override async Task<Module> AddAsync(Module module)
        {
            module.IsActive = true;
            await _context.Module.AddAsync(module);
            await _context.SaveChangesAsync();
            return module;
        }

        public override async Task<bool> UpdateAsync(Module module)
        {
            var existingModule = await _context.Module.FindAsync(module.Id);
            if (existingModule == null)
                return false;

            _context.Entry(existingModule).CurrentValues.SetValues(module);
            await _context.SaveChangesAsync();
            return true;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var module = await _context.Module.FindAsync(id);
            if (module == null)
                return false;

            _context.Module.Remove(module);
            await _context.SaveChangesAsync();
            return true;
        }

        public override async Task<bool> DeleteLogicalAsync(int id)
        {
            var module = await _context.Module.FindAsync(id);
            if (module == null)
                return false;

            module.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}