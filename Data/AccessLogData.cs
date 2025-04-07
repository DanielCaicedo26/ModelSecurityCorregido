

using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gestión de la entidad AccessLog en la base de datos.
    /// </summary>
    public class AccessLogData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccessLogData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{AccessLogData}"/> para el registro de logs.</param>
        public AccessLogData(ApplicationDbContext context, ILogger<AccessLogData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros de acceso almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de registros de acceso.</returns>
        public async Task<IEnumerable<AccessLog>> GetAllAsync()
        {
            try
            {
                return await _context.Set<AccessLog>()
                    .Include(a => a.User)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los registros de acceso");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un registro de acceso específico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del registro.</param>
        /// <returns>El registro encontrado o null si no existe.</returns>
        public async Task<AccessLog?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<AccessLog>()
                    .Include(a => a.User)
                    .FirstOrDefaultAsync(a => a.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el registro de acceso con ID {AccessLogId}", id);
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo registro de acceso en la base de datos.
        /// </summary>
        /// <param name="accessLog">Instancia del registro a crear.</param>
        /// <returns>El registro creado.</returns>
        public async Task<AccessLog> CreateAsync(AccessLog accessLog)
        {
            try
            {
                await _context.Set<AccessLog>().AddAsync(accessLog);
                await _context.SaveChangesAsync();
                return accessLog;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el registro de acceso");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un registro de acceso existente en la base de datos.
        /// </summary>
        /// <param name="accessLog">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(AccessLog accessLog)
        {
            try
            {
                var existingLog = await _context.Set<AccessLog>().FindAsync(accessLog.Id);
                if (existingLog == null)
                {
                    _logger.LogWarning("No se encontró el registro de acceso con ID {AccessLogId} para actualizar", accessLog.Id);
                    return false;
                }

                _context.Entry(existingLog).CurrentValues.SetValues(accessLog);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el registro de acceso");
                return false;
            }
        }

        /// <summary>
        /// Elimina un registro de acceso de la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del registro a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un registro de acceso con ID inválido: {AccessLogId}", id);
                return false;
            }

            try
            {
                var log = await _context.Set<AccessLog>().FindAsync(id);
                if (log == null)
                {
                    _logger.LogInformation("No se encontró ningún registro de acceso con ID: {AccessLogId}", id);
                    return false;
                }

                _context.Set<AccessLog>().Remove(log);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el registro de acceso con ID {AccessLogId}", id);
                return false;
            }
        }
    }
}
