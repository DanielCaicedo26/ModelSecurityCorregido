using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Web2.Services;

namespace Data
{
    public class StateInfractionData
    {
        private readonly IDbContextProvider _dbContextProvider;
        private readonly ILogger<StateInfractionData> _logger;

        public StateInfractionData(IDbContextProvider dbContextProvider, ILogger<StateInfractionData> logger)
        {
            _dbContextProvider = dbContextProvider;
            _logger = logger;
        }

        private ApplicationDbContext _context => _dbContextProvider.GetDbContext();

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
                _logger.LogError(ex, "Error al obtener infracciones con número de documento {DocumentNumber}", documentNumber);
                throw;
            }
        }

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