using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entity.Services;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gesti�n de la entidad TypePayment en la base de datos.
    /// </summary>
    public class TypePaymentData
    {
        private readonly IDynamicDbContextService _dynamicContext;
        private ApplicationDbContext _context => _dynamicContext.GetCurrentApplicationContext();
        private readonly ILogger<TypePaymentData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexi�n con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{TypePaymentData}"/> para el registro de logs.</param>
        public TypePaymentData(IDynamicDbContextService dynamicContext, ILogger<TypePaymentData> logger)
        {
            _dynamicContext = dynamicContext;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los tipos de pago almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de tipos de pago.</returns>
        public async Task<IEnumerable<TypePayment>> GetAllAsync()
        {
            try
            {
                return await _context.Set<TypePayment>()
                    .Include(tp => tp.PaymentUsers)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los tipos de pago");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un tipo de pago espec�fico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del tipo de pago.</param>
        /// <returns>El tipo de pago encontrado o null si no existe.</returns>
        public async Task<TypePayment?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<TypePayment>()
                    .Include(tp => tp.PaymentUsers)
                    .FirstOrDefaultAsync(tp => tp.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el tipo de pago con ID {TypePaymentId}", id);
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo tipo de pago en la base de datos.
        /// </summary>
        /// <param name="typePayment">Instancia del tipo de pago a crear.</param>
        /// <returns>El tipo de pago creado.</returns>
        public async Task<TypePayment> CreateAsync(TypePayment typePayment)
        {
            try
            {
                await _context.Set<TypePayment>().AddAsync(typePayment);
                await _context.SaveChangesAsync();
                return typePayment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el tipo de pago");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un tipo de pago existente en la base de datos.
        /// </summary>
        /// <param name="typePayment">Objeto con la informaci�n actualizada.</param>
        /// <returns>True si la operaci�n fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(TypePayment typePayment)
        {
            try
            {
                var existingTypePayment = await _context.Set<TypePayment>().FindAsync(typePayment.Id);
                if (existingTypePayment == null)
                {
                    _logger.LogWarning("No se encontr� el tipo de pago con ID {TypePaymentId} para actualizar", typePayment.Id);
                    return false;
                }

                _context.Entry(existingTypePayment).CurrentValues.SetValues(typePayment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el tipo de pago");
                return false;
            }
        }

        /// <summary>
        /// Elimina un tipo de pago de la base de datos.
        /// </summary>
        /// <param name="id">Identificador �nico del tipo de pago a eliminar.</param>
        /// <returns>True si la eliminaci�n fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� eliminar un tipo de pago con ID inv�lido: {TypePaymentId}", id);
                return false;
            }

            try
            {
                var typePayment = await _context.Set<TypePayment>().FindAsync(id);
                if (typePayment == null)
                {
                    _logger.LogInformation("No se encontr� ning�n tipo de pago con ID: {TypePaymentId}", id);
                    return false;
                }

                _context.Set<TypePayment>().Remove(typePayment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el tipo de pago con ID {TypePaymentId}", id);
                return false;
            }
        }
    }
}





