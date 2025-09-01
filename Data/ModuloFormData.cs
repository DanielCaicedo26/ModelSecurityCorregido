using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entity.Services;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gesti�n de la entidad ModuloForm en la base de datos.
    /// </summary>
    public class ModuloFormData
    {
        private readonly IDynamicDbContextService _dynamicContext;
        private ApplicationDbContext _context => _dynamicContext.GetCurrentApplicationContext();
        private readonly ILogger<ModuloFormData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexi�n con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{ModuloFormData}"/> para el registro de logs.</param>
        public ModuloFormData(IDynamicDbContextService dynamicContext, ILogger<ModuloFormData> logger)
        {
            _dynamicContext = dynamicContext;
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
        /// Obtiene un ModuloForm espec�fico por su identificador.
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
        /// <param name="moduloForm">Objeto con la informaci�n actualizada.</param>
        /// <returns>True si la operaci�n fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(ModuloForm moduloForm)
        {
            try
            {
                var existingModuloForm = await _context.Set<ModuloForm>().FindAsync(moduloForm.Id);
                if (existingModuloForm == null)
                {
                    _logger.LogWarning("No se encontr� el ModuloForm con ID {ModuloFormId} para actualizar", moduloForm.Id);
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
        /// <param name="id">Identificador �nico del ModuloForm a eliminar.</param>
        /// <returns>True si la eliminaci�n fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� eliminar un ModuloForm con ID inv�lido: {ModuloFormId}", id);
                return false;
            }

            try
            {
                var moduloForm = await _context.Set<ModuloForm>().FindAsync(id);
                if (moduloForm == null)
                {
                    _logger.LogInformation("No se encontr� ning�n ModuloForm con ID: {ModuloFormId}", id);
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


