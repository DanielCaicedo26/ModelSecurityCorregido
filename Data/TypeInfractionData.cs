using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Data.Interfaces;

namespace Data
{
    public class TypeInfractionData
    {
        private readonly IDbContextProvider _dbContextProvider;
        private readonly ILogger<TypeInfractionData> _logger;

        public TypeInfractionData(IDbContextProvider dbContextProvider, ILogger<TypeInfractionData> logger)
        {
            _dbContextProvider = dbContextProvider;
            _logger = logger;
        }

        private ApplicationDbContext _context => _dbContextProvider.GetDbContext();

        public async Task<IEnumerable<TypeInfraction>> GetAllAsync()
        {
            try
            {
                return await _context.Set<TypeInfraction>()
                    .Include(ti => ti.User)
                    .Include(ti => ti.StateInfraction)
                    .Include(ti => ti.InformationInfraction)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las infracciones de tipo");
                throw;
            }
        }

        public async Task<TypeInfraction?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<TypeInfraction>()
                    .Include(ti => ti.User)
                    .Include(ti => ti.StateInfraction)
                    .Include(ti => ti.InformationInfraction)
                    .FirstOrDefaultAsync(ti => ti.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la infracción de tipo con ID {TypeInfractionId}", id);
                throw;
            }
        }

        public async Task<TypeInfraction> CreateAsync(TypeInfraction typeInfraction)
        {
            try
            {
                await _context.Set<TypeInfraction>().AddAsync(typeInfraction);
                await _context.SaveChangesAsync();
                return typeInfraction;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la infracción de tipo");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(TypeInfraction typeInfraction)
        {
            try
            {
                var existingTypeInfraction = await _context.Set<TypeInfraction>().FindAsync(typeInfraction.Id);
                if (existingTypeInfraction == null)
                {
                    _logger.LogWarning("No se encontró la infracción de tipo con ID {TypeInfractionId} para actualizar", typeInfraction.Id);
                    return false;
                }

                _context.Entry(existingTypeInfraction).CurrentValues.SetValues(typeInfraction);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la infracción de tipo");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar una infracción de tipo con ID inválido: {TypeInfractionId}", id);
                return false;
            }

            try
            {
                var typeInfraction = await _context.Set<TypeInfraction>().FindAsync(id);
                if (typeInfraction == null)
                {
                    _logger.LogInformation("No se encontró ninguna infracción de tipo con ID: {TypeInfractionId}", id);
                    return false;
                }

                _context.Set<TypeInfraction>().Remove(typeInfraction);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la infracción de tipo con ID {TypeInfractionId}", id);
                return false;
            }
        }
    }
}