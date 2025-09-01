using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Web2.Services;

namespace Data
{
    public class TypePaymentData
    {
        private readonly IDbContextProvider _dbContextProvider;
        private readonly ILogger<TypePaymentData> _logger;

        public TypePaymentData(IDbContextProvider dbContextProvider, ILogger<TypePaymentData> logger)
        {
            _dbContextProvider = dbContextProvider;
            _logger = logger;
        }

        private ApplicationDbContext _context => _dbContextProvider.GetDbContext();

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

        public async Task<bool> UpdateAsync(TypePayment typePayment)
        {
            try
            {
                var existingTypePayment = await _context.Set<TypePayment>().FindAsync(typePayment.Id);
                if (existingTypePayment == null)
                {
                    _logger.LogWarning("No se encontró el tipo de pago con ID {TypePaymentId} para actualizar", typePayment.Id);
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

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un tipo de pago con ID inválido: {TypePaymentId}", id);
                return false;
            }

            try
            {
                var typePayment = await _context.Set<TypePayment>().FindAsync(id);
                if (typePayment == null)
                {
                    _logger.LogInformation("No se encontró ningún tipo de pago con ID: {TypePaymentId}", id);
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