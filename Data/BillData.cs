using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Web2.Services;

namespace Data
{
    public class BillData
    {
        private readonly IDbContextProvider _dbContextProvider;
        private readonly ILogger<BillData> _logger;

        public BillData(IDbContextProvider dbContextProvider, ILogger<BillData> logger)
        {
            _dbContextProvider = dbContextProvider;
            _logger = logger;
        }

        private ApplicationDbContext _context => _dbContextProvider.GetDbContext();

        public async Task<IEnumerable<Bill>> GetAllAsync()
        {
            try
            {
                return await _context.Set<Bill>()
                    .Include(b => b.PaymentAgreement)
                    .Include(b => b.PaymentUsers)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las facturas");
                throw;
            }
        }

        public async Task<Bill?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Bill>()
                    .Include(b => b.PaymentAgreement)
                    .Include(b => b.PaymentUsers)
                    .FirstOrDefaultAsync(b => b.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la factura con ID {BillId}", id);
                throw;
            }
        }

        public async Task<Bill> CreateAsync(Bill bill)
        {
            try
            {
                await _context.Set<Bill>().AddAsync(bill);
                await _context.SaveChangesAsync();
                return bill;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la factura");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Bill bill)
        {
            try
            {
                var existingBill = await _context.Set<Bill>().FindAsync(bill.Id);
                if (existingBill == null)
                {
                    _logger.LogWarning("No se encontró la factura con ID {BillId} para actualizar", bill.Id);
                    return false;
                }

                _context.Entry(existingBill).CurrentValues.SetValues(bill);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la factura");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar una factura con ID inválido: {BillId}", id);
                return false;
            }

            try
            {
                var bill = await _context.Set<Bill>().FindAsync(id);
                if (bill == null)
                {
                    _logger.LogInformation("No se encontró ninguna factura con ID: {BillId}", id);
                    return false;
                }

                _context.Set<Bill>().Remove(bill);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la factura con ID {BillId}", id);
                return false;
            }
        }
    }
}