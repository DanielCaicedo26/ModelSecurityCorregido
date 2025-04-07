using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gestión de la entidad PaymentHistory en la base de datos.
    /// </summary>
    public class PaymentHistoryData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PaymentHistoryData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{PaymentHistoryData}"/> para el registro de logs.</param>
        public PaymentHistoryData(ApplicationDbContext context, ILogger<PaymentHistoryData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los historiales de pago almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de historiales de pago.</returns>
        public async Task<IEnumerable<PaymentHistory>> GetAllAsync()
        {
            try
            {
                return await _context.Set<PaymentHistory>()
                    .Include(ph => ph.User)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los historiales de pago");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un historial de pago específico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del historial de pago.</param>
        /// <returns>El historial de pago encontrado o null si no existe.</returns>
        public async Task<PaymentHistory?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<PaymentHistory>()
                    .Include(ph => ph.User)
                    .FirstOrDefaultAsync(ph => ph.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el historial de pago con ID {PaymentHistoryId}", id);
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo historial de pago en la base de datos.
        /// </summary>
        /// <param name="paymentHistory">Instancia del historial de pago a crear.</param>
        /// <returns>El historial de pago creado.</returns>
        public async Task<PaymentHistory> CreateAsync(PaymentHistory paymentHistory)
        {
            try
            {
                await _context.Set<PaymentHistory>().AddAsync(paymentHistory);
                await _context.SaveChangesAsync();
                return paymentHistory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el historial de pago");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un historial de pago existente en la base de datos.
        /// </summary>
        /// <param name="paymentHistory">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(PaymentHistory paymentHistory)
        {
            try
            {
                var existingPaymentHistory = await _context.Set<PaymentHistory>().FindAsync(paymentHistory.Id);
                if (existingPaymentHistory == null)
                {
                    _logger.LogWarning("No se encontró el historial de pago con ID {PaymentHistoryId} para actualizar", paymentHistory.Id);
                    return false;
                }

                _context.Entry(existingPaymentHistory).CurrentValues.SetValues(paymentHistory);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el historial de pago");
                return false;
            }
        }

        /// <summary>
        /// Elimina un historial de pago de la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del historial de pago a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un historial de pago con ID inválido: {PaymentHistoryId}", id);
                return false;
            }

            try
            {
                var paymentHistory = await _context.Set<PaymentHistory>().FindAsync(id);
                if (paymentHistory == null)
                {
                    _logger.LogInformation("No se encontró ningún historial de pago con ID: {PaymentHistoryId}", id);
                    return false;
                }

                _context.Set<PaymentHistory>().Remove(paymentHistory);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el historial de pago con ID {PaymentHistoryId}", id);
                return false;
            }
        }
    }
}



