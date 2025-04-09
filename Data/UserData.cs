using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gestión de la entidad User en la base de datos.
    /// </summary>
    public class UserData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{UserData}"/> para el registro de logs.</param>
        public UserData(ApplicationDbContext context, ILogger<UserData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los usuarios almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de usuarios.</returns>
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                return await _context.Set<User>()
                    .Include(u => u.Person)
                    .Include(u => u.RoleUsers)
                    .Include(u => u.UserNotifications)
                    .Include(u => u.PaymentHistories)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un usuario específico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del usuario.</param>
        /// <returns>El usuario encontrado o null si no existe.</returns>
        public async Task<User?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<User>()
                    .Include(u => u.Person)
                    .Include(u => u.RoleUsers)
                    .Include(u => u.UserNotifications)
                    .Include(u => u.PaymentHistories)
                    .FirstOrDefaultAsync(u => u.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con ID {UserId}", id);
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo usuario en la base de datos.
        /// </summary>
        /// <param name="user">Instancia del usuario a crear.</param>
        /// <returns>El usuario creado.</returns>
        public async Task<User> CreateAsync(User user)
        {
            try
            {
                await _context.Set<User>().AddAsync(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el usuario");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un usuario existente en la base de datos.
        /// </summary>
        /// <param name="user">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(User user)
        {
            try
            {
                var existingUser = await _context.Set<User>().FindAsync(user.Id);
                if (existingUser == null)
                {
                    _logger.LogWarning("No se encontró el usuario con ID {UserId} para actualizar", user.Id);
                    return false;
                }

                _context.Entry(existingUser).CurrentValues.SetValues(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario");
                return false;
            }
        }

        /// <summary>
        /// Elimina un usuario de la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del usuario a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un usuario con ID inválido: {UserId}", id);
                return false;
            }

            try
            {
                var user = await _context.Set<User>().FindAsync(id);
                if (user == null)
                {
                    _logger.LogInformation("No se encontró ningún usuario con ID: {UserId}", id);
                    return false;
                }

                _context.Set<User>().Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el usuario con ID {UserId}", id);
                return false;
            }
        }
    }
}






