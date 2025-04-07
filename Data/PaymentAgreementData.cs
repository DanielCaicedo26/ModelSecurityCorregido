using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gestión de la entidad PaymentAgreement en la base de datos.
    /// </summary>
    public class PaymentAgreementData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PaymentAgreementData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{PaymentAgreementData}"/> para el registro de logs.</param>
        public PaymentAgreementData(ApplicationDbContext context, ILogger<PaymentAgreementData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los acuerdos de pago almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de acuerdos de pago.</returns>
        public async Task<IEnumerable<PaymentAgreement>> GetAllAsync()
        {
            try
            {
                return await _context.Set<PaymentAgreement>()
                    .Include(pa => pa.PaymentUsers)
                    .Include(pa => pa.Bills)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los acuerdos de pago");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un acuerdo de pago específico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del acuerdo de pago.</param>
        /// <returns>El acuerdo de pago encontrado o null si no existe.</returns>
        public async Task<PaymentAgreement?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<PaymentAgreement>()
                    .Include(pa => pa.PaymentUsers)
                    .Include(pa => pa.Bills)
                    .FirstOrDefaultAsync(pa => pa.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el acuerdo de pago con ID {PaymentAgreementId}", id);
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo acuerdo de pago en la base de datos.
        /// </summary>
        /// <param name="paymentAgreement">Instancia del acuerdo de pago a crear.</param>
        /// <returns>El acuerdo de pago creado.</returns>
        public async Task<PaymentAgreement> CreateAsync(PaymentAgreement paymentAgreement)
        {
            try
            {
                await _context.Set<PaymentAgreement>().AddAsync(paymentAgreement);
                await _context.SaveChangesAsync();
                return paymentAgreement;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el acuerdo de pago");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un acuerdo de pago existente en la base de datos.
        /// </summary>
        /// <param name="paymentAgreement">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(PaymentAgreement paymentAgreement)
        {
            try
            {
                var existingPaymentAgreement = await _context.Set<PaymentAgreement>().FindAsync(paymentAgreement.Id);
                if (existingPaymentAgreement == null)
                {
                    _logger.LogWarning("No se encontró el acuerdo de pago con ID {PaymentAgreementId} para actualizar", paymentAgreement.Id);
                    return false;
                }

                _context.Entry(existingPaymentAgreement).CurrentValues.SetValues(paymentAgreement);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el acuerdo de pago");
                return false;
            }
        }

        /// <summary>
        /// Elimina un acuerdo de pago de la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del acuerdo de pago a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un acuerdo de pago con ID inválido: {PaymentAgreementId}", id);
                return false;
            }

            try
            {
                var paymentAgreement = await _context.Set<PaymentAgreement>().FindAsync(id);
                if (paymentAgreement == null)
                {
                    _logger.LogInformation("No se encontró ningún acuerdo de pago con ID: {PaymentAgreementId}", id);
                    return false;
                }

                _context.Set<PaymentAgreement>().Remove(paymentAgreement);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el acuerdo de pago con ID {PaymentAgreementId}", id);
                return false;
            }
        }
    }
}


