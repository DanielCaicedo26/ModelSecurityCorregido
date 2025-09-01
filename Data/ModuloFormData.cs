using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Data.Interfaces;

namespace Data
{
    public class ModuloFormData
    {
        private readonly IDbContextProvider _dbContextProvider;
        private readonly ILogger<ModuloFormData> _logger;

        public ModuloFormData(IDbContextProvider dbContextProvider, ILogger<ModuloFormData> logger)
        {
            _dbContextProvider = dbContextProvider;
            _logger = logger;
        }

        private ApplicationDbContext _context => _dbContextProvider.GetDbContext();

        public async Task<IEnumerable<ModuloForm>> GetAllAsync()
        {
            try
            {
                return await _context.Set<ModuloForm>()
                    .Include(mf => mf.Form)
                    .Include(mf => mf.Module)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los ModuloForm");
                throw;
            }
        }

        public async Task<ModuloForm?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<ModuloForm>()
                    .Include(mf => mf.Form)
                    .Include(mf => mf.Module)
                    .FirstOrDefaultAsync(mf => mf.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el ModuloForm con ID {ModuloFormId}", id);
                throw;
            }
        }

        public async Task<ModuloForm> CreateAsync(ModuloForm moduloForm)
        {
            try
            {
                await _context.Set<ModuloForm>().AddAsync(moduloForm);
                await _context.SaveChangesAsync();
                return moduloForm;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el ModuloForm");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(ModuloForm moduloForm)
        {
            try
            {
                var existingModuloForm = await _context.Set<ModuloForm>().FindAsync(moduloForm.Id);
                if (existingModuloForm == null)
                {
                    _logger.LogWarning("No se encontró el ModuloForm con ID {ModuloFormId} para actualizar", moduloForm.Id);
                    return false;
                }

                _context.Entry(existingModuloForm).CurrentValues.SetValues(moduloForm);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el ModuloForm");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un ModuloForm con ID inválido: {ModuloFormId}", id);
                return false;
            }

            try
            {
                var moduloForm = await _context.Set<ModuloForm>().FindAsync(id);
                if (moduloForm == null)
                {
                    _logger.LogInformation("No se encontró ningún ModuloForm con ID: {ModuloFormId}", id);
                    return false;
                }

                _context.Set<ModuloForm>().Remove(moduloForm);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el ModuloForm con ID {ModuloFormId}", id);
                return false;
            }
        }
    }
}