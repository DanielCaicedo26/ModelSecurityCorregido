using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entity.Services;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gesti�n de la entidad StateInfraction en la base de datos.
    /// </summary>
    public class StateInfractionData
    {
        private readonly IDynamicDbContextService _dynamicContext;
        private ApplicationDbContext _context => _dynamicContext.GetCurrentApplicationContext();
        private readonly ILogger<StateInfractionData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexi�n con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{StateInfractionData}"/> para el registro de logs.</param>
        public StateInfractionData(IDynamicDbContextService dynamicContext, ILogger<StateInfractionData> logger)
        {
            _dynamicContext = dynamicContext;
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
        /// Obtiene una infracci�n de estado espec�fica por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la infracci�n de estado.</param>
        /// <returns>La infracci�n de estado encontrada o null si no existe.</returns>
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
                _logger.LogError(ex, "Error al obtener la infracci�n de estado con ID {StateInfractionId}", id);
                throw;
            }
        }

        /// <summary>
        /// Obtiene infracciones por n�mero de documento.
        /// </summary>
        /// <param name="documentNumber">N�mero de documento a buscar.</param>
        /// <returns>Lista de infracciones con el n�mero de documento especificado.</returns>
        public async Task<IEnumerable<StateInfraction>> GetByDocumentNumberAsync(string documentNumber)
        {
            try
            {
                return await _context.Set<StateInfraction>()
                    .Include(s => s.Person)
                    .Include(s => s.Infraction)
                    .Where(s => s.DocumentNumber == documentNumber)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener infracciones con n�mero de documento {DocumentNumber}", documentNumber);
                throw;
            }
        }

        /// <summary>
        /// Obtiene infracciones por IDs de personas.
        /// </summary>
        /// <param name="personIds">Lista de IDs de personas.</param>
        /// <returns>Lista de infracciones asociadas a las personas especificadas.</returns>
        public async Task<IEnumerable<StateInfraction>> GetByPersonIdsAsync(IEnumerable<int> personIds)
        {
            try
            {
                return await _context.Set<StateInfraction>()
                    .Include(s => s.Person)
                    .Include(s => s.Infraction)
                    .Where(s => personIds.Contains(s.PersonId))
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener infracciones por IDs de personas");
                throw;
            }
        }

        /// <summary>
        /// Crea una nueva infracci�n de estado en la base de datos.
        /// </summary>
        /// <param name="stateInfraction">Instancia de la infracci�n de estado a crear.</param>
        /// <returns>La infracci�n de estado creada.</returns>
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
                _logger.LogError(ex, "Error al crear la infracci�n de estado");
                throw;
            }
        }

        /// <summary>
        /// Actualiza una infracci�n de estado existente en la base de datos.
        /// </summary>
        /// <param name="stateInfraction">Objeto con la informaci�n actualizada.</param>
        /// <returns>True si la operaci�n fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(StateInfraction stateInfraction)
        {
            try
            {
                var existingStateInfraction = await _context.Set<StateInfraction>().FindAsync(stateInfraction.Id);
                if (existingStateInfraction == null)
                {
                    _logger.LogWarning("No se encontr� la infracci�n de estado con ID {StateInfractionId} para actualizar", stateInfraction.Id);
                    return false;
                }

                _context.Entry(existingStateInfraction).CurrentValues.SetValues(stateInfraction);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la infracci�n de estado");
                return false;
            }
        }

        /// <summary>
        /// Elimina una infracci�n de estado de la base de datos.
        /// </summary>
        /// <param name="id">Identificador �nico de la infracci�n de estado a eliminar.</param>
        /// <returns>True si la eliminaci�n fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� eliminar una infracci�n de estado con ID inv�lido: {StateInfractionId}", id);
                return false;
            }

            try
            {
                var stateInfraction = await _context.Set<StateInfraction>().FindAsync(id);
                if (stateInfraction == null)
                {
                    _logger.LogInformation("No se encontr� ninguna infracci�n de estado con ID: {StateInfractionId}", id);
                    return false;
                }

                _context.Set<StateInfraction>().Remove(stateInfraction);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la infracci�n de estado con ID {StateInfractionId}", id);
                return false;
            }
        }
    }
}




