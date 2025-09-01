using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Data.Interfaces;

namespace Data
{
    public class PaymentAgreementData
    {
        private readonly IDbContextProvider _dbContextProvider;
        private readonly ILogger<PaymentAgreementData> _logger;

        public PaymentAgreementData(IDbContextProvider dbContextProvider, ILogger<PaymentAgreementData> logger)
        {
            _dbContextProvider = dbContextProvider;
            _logger = logger;
        }

        private ApplicationDbContext _context => _dbContextProvider.GetDbContext();

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