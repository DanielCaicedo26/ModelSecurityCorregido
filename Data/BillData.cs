using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gestión de la entidad Bill en la base de datos.
    /// </summary>
    public class BillData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BillData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{BillData}"/> para el registro de logs.</param>
        public BillData(ApplicationDbContext context, ILogger<BillData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las facturas almacenadas en la base de datos.
        /// </summary>
        /// <returns>Lista de facturas.</returns>
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

        /// <summary>
        /// Obtiene una factura específica por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la factura.</param>
        /// <returns>La factura encontrada o null si no existe.</returns>
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

        /// <summary>
        /// Crea una nueva factura en la base de datos.
        /// </summary>
        /// <param name="bill">Instancia de la factura a crear.</param>
        /// <returns>La factura creada.</returns>
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

        /// <summary>
        /// Actualiza una factura existente en la base de datos.
        /// </summary>
        /// <param name="bill">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
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

        /// <summary>
        /// Elimina una factura de la base de datos.
        /// </summary>
        /// <param name="id">Identificador único de la factura a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
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
