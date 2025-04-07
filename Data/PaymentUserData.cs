using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gestión de la entidad PaymentUser en la base de datos.
    /// </summary>
    public class PaymentUserData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PaymentUserData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{PaymentUserData}"/> para el registro de logs.</param>
        public PaymentUserData(ApplicationDbContext context, ILogger<PaymentUserData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los usuarios de pago almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de usuarios de pago.</returns>
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

        /// <summary>
        /// Obtiene un usuario de pago específico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del usuario de pago.</param>
        /// <returns>El usuario de pago encontrado o null si no existe.</returns>
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

        /// <summary>
        /// Crea un nuevo usuario de pago en la base de datos.
        /// </summary>
        /// <param name="paymentUser">Instancia del usuario de pago a crear.</param>
        /// <returns>El usuario de pago creado.</returns>
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

        /// <summary>
        /// Actualiza un usuario de pago existente en la base de datos.
        /// </summary>
        /// <param name="paymentUser">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
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

        /// <summary>
        /// Elimina un usuario de pago de la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del usuario de pago a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
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
