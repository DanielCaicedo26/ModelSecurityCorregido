using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Web2.Services;

namespace Data
{
    public class PaymentUserData
    {
        private readonly IDbContextProvider _dbContextProvider;
        private readonly ILogger<PaymentUserData> _logger;

        public PaymentUserData(IDbContextProvider dbContextProvider, ILogger<PaymentUserData> logger)
        {
            _dbContextProvider = dbContextProvider;
            _logger = logger;
        }

        private ApplicationDbContext _context => _dbContextProvider.GetDbContext();

        public async Task<IEnumerable<PaymentUser>> GetAllAsync()
        {
            try
            {
                return await _context.Set<PaymentUser>()
                    .Include(pu => pu.Person)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios de pago");
                throw;
            }
        }

        public async Task<PaymentUser?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<PaymentUser>()
                    .Include(pu => pu.Person)
                    .FirstOrDefaultAsync(pu => pu.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario de pago con ID {PaymentUserId}", id);
                throw;
            }
        }

        public async Task<PaymentUser> CreateAsync(PaymentUser paymentUser)
        {
            try
            {
                await _context.Set<PaymentUser>().AddAsync(paymentUser);
                await _context.SaveChangesAsync();
                return paymentUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el usuario de pago");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(PaymentUser paymentUser)
        {
            try
            {
                var existingPaymentUser = await _context.Set<PaymentUser>().FindAsync(paymentUser.Id);
                if (existingPaymentUser == null)
                {
                    _logger.LogWarning("No se encontró el usuario de pago con ID {PaymentUserId} para actualizar", paymentUser.Id);
                    return false;
                }

                _context.Entry(existingPaymentUser).CurrentValues.SetValues(paymentUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario de pago");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un usuario de pago con ID inválido: {PaymentUserId}", id);
                return false;
            }

            try
            {
                var paymentUser = await _context.Set<PaymentUser>().FindAsync(id);
                if (paymentUser == null)
                {
                    _logger.LogInformation("No se encontró ningún usuario de pago con ID: {PaymentUserId}", id);
                    return false;
                }

                _context.Set<PaymentUser>().Remove(paymentUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el usuario de pago con ID {PaymentUserId}", id);
                return false;
            }
        }
    }
}