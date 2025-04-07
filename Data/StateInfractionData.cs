using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gestión de la entidad StateInfraction en la base de datos.
    /// </summary>
    public class StateInfractionData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StateInfractionData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{StateInfractionData}"/> para el registro de logs.</param>
        public StateInfractionData(ApplicationDbContext context, ILogger<StateInfractionData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las infracciones de estado almacenadas en la base de datos.
        /// </summary>
        /// <returns>Lista de infracciones de estado.</returns>
        public async Task<IEnumerable<StateInfraction>> GetAllAsync()
        {
            try
            {
                return await _context.Set<StateInfraction>()
                    .Include(si => si.Person)
                    .Include(si => si.Infraction)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las infracciones de estado");
                throw;
            }
        }

        /// <summary>
        /// Obtiene una infracción de estado específica por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la infracción de estado.</param>
        /// <returns>La infracción de estado encontrada o null si no existe.</returns>
        public async Task<StateInfraction?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<StateInfraction>()
                    .Include(si => si.Person)
                    .Include(si => si.Infraction)
                    .FirstOrDefaultAsync(si => si.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la infracción de estado con ID {StateInfractionId}", id);
                throw;
            }
        }

        /// <summary>
        /// Crea una nueva infracción de estado en la base de datos.
        /// </summary>
        /// <param name="stateInfraction">Instancia de la infracción de estado a crear.</param>
        /// <returns>La infracción de estado creada.</returns>
        public async Task<StateInfraction> CreateAsync(StateInfraction stateInfraction)
        {
            try
            {
                await _context.Set<StateInfraction>().AddAsync(stateInfraction);
                await _context.SaveChangesAsync();
                return stateInfraction;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la infracción de estado");
                throw;
            }
        }

        /// <summary>
        /// Actualiza una infracción de estado existente en la base de datos.
        /// </summary>
        /// <param name="stateInfraction">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(StateInfraction stateInfraction)
        {
            try
            {
                var existingStateInfraction = await _context.Set<StateInfraction>().FindAsync(stateInfraction.Id);
                if (existingStateInfraction == null)
                {
                    _logger.LogWarning("No se encontró la infracción de estado con ID {StateInfractionId} para actualizar", stateInfraction.Id);
                    return false;
                }

                _context.Entry(existingStateInfraction).CurrentValues.SetValues(stateInfraction);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la infracción de estado");
                return false;
            }
        }

        /// <summary>
        /// Elimina una infracción de estado de la base de datos.
        /// </summary>
        /// <param name="id">Identificador único de la infracción de estado a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar una infracción de estado con ID inválido: {StateInfractionId}", id);
                return false;
            }

            try
            {
                var stateInfraction = await _context.Set<StateInfraction>().FindAsync(id);
                if (stateInfraction == null)
                {
                    _logger.LogInformation("No se encontró ninguna infracción de estado con ID: {StateInfractionId}", id);
                    return false;
                }

                _context.Set<StateInfraction>().Remove(stateInfraction);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la infracción de estado con ID {StateInfractionId}", id);
                return false;
            }
        }
    }
}




