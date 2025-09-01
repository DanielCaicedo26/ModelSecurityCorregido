using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entity.Services;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gesti�n de la entidad UserNotification en la base de datos.
    /// </summary>
    public class UserNotificationData
    {
        private readonly IDynamicDbContextService _dynamicContext;
        private ApplicationDbContext _context => _dynamicContext.GetCurrentApplicationContext();
        private readonly ILogger<UserNotificationData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexi�n con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{UserNotificationData}"/> para el registro de logs.</param>
        public UserNotificationData(IDynamicDbContextService dynamicContext, ILogger<UserNotificationData> logger)
        {
            _dynamicContext = dynamicContext;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las notificaciones de usuario almacenadas en la base de datos.
        /// </summary>
        /// <returns>Lista de notificaciones de usuario.</returns>
        public async Task<IEnumerable<UserNotification>> GetAllAsync()
        {
            try
            {
                return await _context.Set<UserNotification>()
                    .Include(un => un.User)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las notificaciones de usuario");
                throw;
            }
        }

        /// <summary>
        /// Obtiene una notificaci�n de usuario espec�fica por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la notificaci�n de usuario.</param>
        /// <returns>La notificaci�n de usuario encontrada o null si no existe.</returns>
        public async Task<UserNotification?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<UserNotification>()
                    .Include(un => un.User)
                    .FirstOrDefaultAsync(un => un.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la notificaci�n de usuario con ID {UserNotificationId}", id);
                throw;
            }
        }

        /// <summary>
        /// Crea una nueva notificaci�n de usuario en la base de datos.
        /// </summary>
        /// <param name="userNotification">Instancia de la notificaci�n de usuario a crear.</param>
        /// <returns>La notificaci�n de usuario creada.</returns>
        public async Task<UserNotification> CreateAsync(UserNotification userNotification)
        {
            try
            {
                await _context.Set<UserNotification>().AddAsync(userNotification);
                await _context.SaveChangesAsync();
                return userNotification;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la notificaci�n de usuario");
                throw;
            }
        }

        /// <summary>
        /// Actualiza una notificaci�n de usuario existente en la base de datos.
        /// </summary>
        /// <param name="userNotification">Objeto con la informaci�n actualizada.</param>
        /// <returns>True si la operaci�n fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(UserNotification userNotification)
        {
            try
            {
                var existingUserNotification = await _context.Set<UserNotification>().FindAsync(userNotification.Id);
                if (existingUserNotification == null)
                {
                    _logger.LogWarning("No se encontr� la notificaci�n de usuario con ID {UserNotificationId} para actualizar", userNotification.Id);
                    return false;
                }

                _context.Entry(existingUserNotification).CurrentValues.SetValues(userNotification);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la notificaci�n de usuario");
                return false;
            }
        }

        /// <summary>
        /// Elimina una notificaci�n de usuario de la base de datos.
        /// </summary>
        /// <param name="id">Identificador �nico de la notificaci�n de usuario a eliminar.</param>
        /// <returns>True si la eliminaci�n fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� eliminar una notificaci�n de usuario con ID inv�lido: {UserNotificationId}", id);
                return false;
            }

            try
            {
                var userNotification = await _context.Set<UserNotification>().FindAsync(id);
                if (userNotification == null)
                {
                    _logger.LogInformation("No se encontr� ninguna notificaci�n de usuario con ID: {UserNotificationId}", id);
                    return false;
                }

                _context.Set<UserNotification>().Remove(userNotification);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la notificaci�n de usuario con ID {UserNotificationId}", id);
                return false;
            }
        }


    }
}







