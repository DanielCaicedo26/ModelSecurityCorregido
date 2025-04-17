using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gestión de la entidad InformationInfraction en la base de datos.
    /// </summary>
    public class InformationInfractionData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InformationInfractionData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{InformationInfractionData}"/> para el registro de logs.</param>
        public InformationInfractionData(ApplicationDbContext context, ILogger<InformationInfractionData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las infracciones de información almacenadas en la base de datos.
        /// </summary>
        /// <returns>Lista de infracciones de información.</returns>
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

        /// <summary>
        /// Obtiene una infracción de información específica por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la infracción de información.</param>
        /// <returns>La infracción de información encontrada o null si no existe.</returns>
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

        /// <summary>
        /// Crea una nueva infracción de información en la base de datos.
        /// </summary>
        /// <param name="informationInfraction">Instancia de la infracción de información a crear.</param>
        /// <returns>La infracción de información creada.</returns>
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

        /// <summary>
        /// Actualiza una infracción de información existente en la base de datos.
        /// </summary>
        /// <param name="informationInfraction">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
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

        /// <summary>
        /// Elimina una infracción de información de la base de datos.
        /// </summary>
        /// <param name="id">Identificador único de la infracción de información a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
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
