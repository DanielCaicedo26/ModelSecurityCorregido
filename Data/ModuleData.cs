using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gestión de la entidad Module en la base de datos.
    /// </summary>
    public class ModuleData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ModuleData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{ModuleData}"/> para el registro de logs.</param>
        public ModuleData(ApplicationDbContext context, ILogger<ModuleData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los módulos almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de módulos.</returns>
        public async Task<IEnumerable<Module>> GetAllAsync()
        {
            try
            {
                return await _context.Set<Module>()
                    .Include(m => m.ModuloForms)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los módulos");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un módulo específico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del módulo.</param>
        /// <returns>El módulo encontrado o null si no existe.</returns>
        public async Task<Module?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Module>()
                    .Include(m => m.ModuloForms)
                    .FirstOrDefaultAsync(m => m.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el módulo con ID {ModuleId}", id);
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo módulo en la base de datos.
        /// </summary>
        /// <param name="module">Instancia del módulo a crear.</param>
        /// <returns>El módulo creado.</returns>
        public async Task<Module> CreateAsync(Module module)
        {
            try
            {
                await _context.Set<Module>().AddAsync(module);
                await _context.SaveChangesAsync();
                return module;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el módulo");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un módulo existente en la base de datos.
        /// </summary>
        /// <param name="module">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Module module)
        {
            try
            {
                var existingModule = await _context.Set<Module>().FindAsync(module.Id);
                if (existingModule == null)
                {
                    _logger.LogWarning("No se encontró el módulo con ID {ModuleId} para actualizar", module.Id);
                    return false;
                }

                _context.Entry(existingModule).CurrentValues.SetValues(module);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el módulo");
                return false;
            }
        }

        /// <summary>
        /// Elimina un módulo de la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del módulo a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un módulo con ID inválido: {ModuleId}", id);
                return false;
            }

            try
            {
                var module = await _context.Set<Module>().FindAsync(id);
                if (module == null)
                {
                    _logger.LogInformation("No se encontró ningún módulo con ID: {ModuleId}", id);
                    return false;
                }

                _context.Set<Module>().Remove(module);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el módulo con ID {ModuleId}", id);
                return false;
            }
        }
    }
}

