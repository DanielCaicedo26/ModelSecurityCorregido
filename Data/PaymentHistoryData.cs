using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Web2.Services;

namespace Data
{
    public class PaymentHistoryData
    {
        private readonly IDbContextProvider _dbContextProvider;
        private readonly ILogger<PaymentHistoryData> _logger;

        public PaymentHistoryData(IDbContextProvider dbContextProvider, ILogger<PaymentHistoryData> logger)
        {
            _dbContextProvider = dbContextProvider;
            _logger = logger;
        }

        private ApplicationDbContext _context => _dbContextProvider.GetDbContext();

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