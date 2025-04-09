using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gestión de la entidad Form en la base de datos.
    /// </summary>
    public class FormData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FormData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{FormData}"/> para el registro de logs.</param>
        public FormData(ApplicationDbContext context, ILogger<FormData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los formularios almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de formularios.</returns>
        public async Task<IEnumerable<Form>> GetAllAsync()
        {
            try
            {
                return await _context.Set<Form>()
                    .Include(f => f.RoleFormPermissions)
                    .Include(f => f.ModuloForms)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los formularios");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un formulario específico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del formulario.</param>
        /// <returns>El formulario encontrado o null si no existe.</returns>
        public async Task<Form?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Form>()
                    .Include(f => f.RoleFormPermissions)
                    .Include(f => f.ModuloForms)
                    .FirstOrDefaultAsync(f => f.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el formulario con ID {FormId}", id);
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo formulario en la base de datos.
        /// </summary>
        /// <param name="form">Instancia del formulario a crear.</param>
        /// <returns>El formulario creado.</returns>
        public async Task<Form> CreateAsync(Form form)
        {
            try
            {
                await _context.Set<Form>().AddAsync(form);
                await _context.SaveChangesAsync();
                return form;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el formulario");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un formulario existente en la base de datos.
        /// </summary>
        /// <param name="form">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Form form)
        {
            try
            {
                var existingForm = await _context.Set<Form>().FindAsync(form.Id);
                if (existingForm == null)
                {
                    _logger.LogWarning("No se encontró el formulario con ID {FormId} para actualizar", form.Id);
                    return false;
                }

                _context.Entry(existingForm).CurrentValues.SetValues(form);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el formulario");
                return false;
            }
        }

        /// <summary>
        /// Elimina un formulario de la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del formulario a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un formulario con ID inválido: {FormId}", id);
                return false;
            }

            try
            {
                var form = await _context.Set<Form>().FindAsync(id);
                if (form == null)
                {
                    _logger.LogInformation("No se encontró ningún formulario con ID: {FormId}", id);
                    return false;
                }

                _context.Set<Form>().Remove(form);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el formulario con ID {FormId}", id);
                return false;
            }
        }
    }
}
