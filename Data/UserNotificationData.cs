using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Web2.Services;

namespace Data
{
    public class UserNotificationData
    {
        private readonly IDbContextProvider _dbContextProvider;
        private readonly ILogger<UserNotificationData> _logger;

        public UserNotificationData(IDbContextProvider dbContextProvider, ILogger<UserNotificationData> logger)
        {
            _dbContextProvider = dbContextProvider;
            _logger = logger;
        }

        private ApplicationDbContext _context => _dbContextProvider.GetDbContext();

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
                _logger.LogError(ex, "Error al obtener la notificación de usuario con ID {UserNotificationId}", id);
                throw;
            }
        }

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
                _logger.LogError(ex, "Error al crear la notificación de usuario");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(UserNotification userNotification)
        {
            try
            {
                var existingUserNotification = await _context.Set<UserNotification>().FindAsync(userNotification.Id);
                if (existingUserNotification == null)
                {
                    _logger.LogWarning("No se encontró la notificación de usuario con ID {UserNotificationId} para actualizar", userNotification.Id);
                    return false;
                }

                _context.Entry(existingUserNotification).CurrentValues.SetValues(userNotification);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la notificación de usuario");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar una notificación de usuario con ID inválido: {UserNotificationId}", id);
                return false;
            }

            try
            {
                var userNotification = await _context.Set<UserNotification>().FindAsync(id);
                if (userNotification == null)
                {
                    _logger.LogInformation("No se encontró ninguna notificación de usuario con ID: {UserNotificationId}", id);
                    return false;
                }

                _context.Set<UserNotification>().Remove(userNotification);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la notificación de usuario con ID {UserNotificationId}", id);
                return false;
            }
        }
    }
}