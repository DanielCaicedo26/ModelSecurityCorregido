using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entity.Services;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gesti�n de la entidad TypeInfraction en la base de datos.
    /// </summary>
    public class TypeInfractionData
    {
        private readonly IDynamicDbContextService _dynamicContext;
        private ApplicationDbContext _context => _dynamicContext.GetCurrentApplicationContext();
        private readonly ILogger<TypeInfractionData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexi�n con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{TypeInfractionData}"/> para el registro de logs.</param>
        public TypeInfractionData(IDynamicDbContextService dynamicContext, ILogger<TypeInfractionData> logger)
        {
            _dynamicContext = dynamicContext;
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

        /// <summary>
        /// Obtiene una infracci�n de tipo espec�fica por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la infracci�n de tipo.</param>
        /// <returns>La infracci�n de tipo encontrada o null si no existe.</returns>
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
                _logger.LogError(ex, "Error al obtener la infracci�n de tipo con ID {TypeInfractionId}", id);
                throw;
            }
        }

        /// <summary>
        /// Crea una nueva infracci�n de tipo en la base de datos.
        /// </summary>
        /// <param name="typeInfraction">Instancia de la infracci�n de tipo a crear.</param>
        /// <returns>La infracci�n de tipo creada.</returns>
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
                _logger.LogError(ex, "Error al crear la infracci�n de tipo");
                throw;
            }
        }

        /// <summary>
        /// Actualiza una infracci�n de tipo existente en la base de datos.
        /// </summary>
        /// <param name="typeInfraction">Objeto con la informaci�n actualizada.</param>
        /// <returns>True si la operaci�n fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(TypeInfraction typeInfraction)
        {
            try
            {
                var existingTypeInfraction = await _context.Set<TypeInfraction>().FindAsync(typeInfraction.Id);
                if (existingTypeInfraction == null)
                {
                    _logger.LogWarning("No se encontr� la infracci�n de tipo con ID {TypeInfractionId} para actualizar", typeInfraction.Id);
                    return false;
                }

                _context.Entry(existingTypeInfraction).CurrentValues.SetValues(typeInfraction);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la infracci�n de tipo");
                return false;
            }
        }

        /// <summary>
        /// Elimina una infracci�n de tipo de la base de datos.
        /// </summary>
        /// <param name="id">Identificador �nico de la infracci�n de tipo a eliminar.</param>
        /// <returns>True si la eliminaci�n fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� eliminar una infracci�n de tipo con ID inv�lido: {TypeInfractionId}", id);
                return false;
            }

            try
            {
                var typeInfraction = await _context.Set<TypeInfraction>().FindAsync(id);
                if (typeInfraction == null)
                {
                    _logger.LogInformation("No se encontr� ninguna infracci�n de tipo con ID: {TypeInfractionId}", id);
                    return false;
                }

                _context.Set<TypeInfraction>().Remove(typeInfraction);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la infracci�n de tipo con ID {TypeInfractionId}", id);
                return false;
            }
        }
    }
}





