using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gestión de la entidad TypeInfraction en la base de datos.
    /// </summary>
    public class TypeInfractionData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TypeInfractionData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{TypeInfractionData}"/> para el registro de logs.</param>
        public TypeInfractionData(ApplicationDbContext context, ILogger<TypeInfractionData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las infracciones de tipo almacenadas en la base de datos.
        /// </summary>
        /// <returns>Lista de infracciones de tipo.</returns>
        public async Task<IEnumerable<TypeInfraction>> GetAllAsync()
        {
            try
            {
                return await _context.Set<TypeInfraction>()
                    .Include(ti => ti.User)
                    .Include(ti => ti.StateInfractions)
                    .Include(ti => ti.InformationInfractions)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las infracciones de tipo");
                throw;
            }
        }

        /// <summary>
        /// Obtiene una infracción de tipo específica por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la infracción de tipo.</param>
        /// <returns>La infracción de tipo encontrada o null si no existe.</returns>
        public async Task<TypeInfraction?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<TypeInfraction>()
                    .Include(ti => ti.User)
                    .Include(ti => ti.StateInfractions)
                    .Include(ti => ti.InformationInfractions)
                    .FirstOrDefaultAsync(ti => ti.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la infracción de tipo con ID {TypeInfractionId}", id);
                throw;
            }
        }

        /// <summary>
        /// Crea una nueva infracción de tipo en la base de datos.
        /// </summary>
        /// <param name="typeInfraction">Instancia de la infracción de tipo a crear.</param>
        /// <returns>La infracción de tipo creada.</returns>
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

        /// <summary>
        /// Actualiza una infracción de tipo existente en la base de datos.
        /// </summary>
        /// <param name="typeInfraction">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
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

        /// <summary>
        /// Elimina una infracción de tipo de la base de datos.
        /// </summary>
        /// <param name="id">Identificador único de la infracción de tipo a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
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





