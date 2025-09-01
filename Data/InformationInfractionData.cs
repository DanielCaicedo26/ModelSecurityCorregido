using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Data.Interfaces;

namespace Data
{
    public class InformationInfractionData
    {
        private readonly IDbContextProvider _dbContextProvider;
        private readonly ILogger<InformationInfractionData> _logger;

        public InformationInfractionData(IDbContextProvider dbContextProvider, ILogger<InformationInfractionData> logger)
        {
            _dbContextProvider = dbContextProvider;
            _logger = logger;
        }

        private ApplicationDbContext _context => _dbContextProvider.GetDbContext();

        public async Task<IEnumerable<InformationInfraction>> GetAllAsync()
        {
            try
            {
                return await _context.Set<InformationInfraction>()
                    .Include(ii => ii.TypeInfraction)
                    .Include(ii => ii.PaymentHistory)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las infracciones de información");
                throw;
            }
        }

        public async Task<InformationInfraction?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<InformationInfraction>()
                    .Include(ii => ii.TypeInfraction)
                    .Include(ii => ii.PaymentHistory)
                    .FirstOrDefaultAsync(ii => ii.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la infracción de información con ID {InformationInfractionId}", id);
                throw;
            }
        }

        public async Task<InformationInfraction> CreateAsync(InformationInfraction informationInfraction)
        {
            try
            {
                await _context.Set<InformationInfraction>().AddAsync(informationInfraction);
                await _context.SaveChangesAsync();
                return informationInfraction;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la infracción de información");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(InformationInfraction informationInfraction)
        {
            try
            {
                var existingInformationInfraction = await _context.Set<InformationInfraction>().FindAsync(informationInfraction.Id);
                if (existingInformationInfraction == null)
                {
                    _logger.LogWarning("No se encontró la infracción de información con ID {InformationInfractionId} para actualizar", informationInfraction.Id);
                    return false;
                }

                _context.Entry(existingInformationInfraction).CurrentValues.SetValues(informationInfraction);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la infracción de información");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar una infracción de información con ID inválido: {InformationInfractionId}", id);
                return false;
            }

            try
            {
                var informationInfraction = await _context.Set<InformationInfraction>().FindAsync(id);
                if (informationInfraction == null)
                {
                    _logger.LogInformation("No se encontró ninguna infracción de información con ID: {InformationInfractionId}", id);
                    return false;
                }

                _context.Set<InformationInfraction>().Remove(informationInfraction);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la infracción de información con ID {InformationInfractionId}", id);
                return false;
            }
        }
    }
}