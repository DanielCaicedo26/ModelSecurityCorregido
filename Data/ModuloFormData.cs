using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gestión de la entidad ModuloForm en la base de datos.
    /// </summary>
    public class ModuloFormData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ModuloFormData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{ModuloFormData}"/> para el registro de logs.</param>
        public ModuloFormData(ApplicationDbContext context, ILogger<ModuloFormData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los ModuloForm almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de ModuloForm.</returns>
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

        /// <summary>
        /// Obtiene un ModuloForm específico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del ModuloForm.</param>
        /// <returns>El ModuloForm encontrado o null si no existe.</returns>
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

        /// <summary>
        /// Crea un nuevo ModuloForm en la base de datos.
        /// </summary>
        /// <param name="moduloForm">Instancia del ModuloForm a crear.</param>
        /// <returns>El ModuloForm creado.</returns>
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

        /// <summary>
        /// Actualiza un ModuloForm existente en la base de datos.
        /// </summary>
        /// <param name="moduloForm">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
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

        /// <summary>
        /// Elimina un ModuloForm de la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del ModuloForm a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
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


