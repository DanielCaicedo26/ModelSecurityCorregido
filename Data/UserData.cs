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
                // Primero obtenemos los usuarios sin incluir propiedades de navegación
                var users = await _context.Set<User>()
                    .AsNoTracking()
                    .ToListAsync();

                // Asignamos valores predeterminados a campos NULL
                foreach (var user in users)
                {
                    if (user.Username == null)
                    {
                        user.Username = $"usuario-{user.Id}";
                    }
                    if (user.Email == null)
                    {
                        user.Email = $"email-{user.Id}@ejemplo.com";
                    }
                    if (user.Password == null)
                    {
                        user.Password = "password-placeholder";
                    }
                }

                // Cargar las propiedades de navegación
                var userIds = users.Select(u => u.Id).ToList();

                // Cargar Person
                var personsWithUsers = await _context.Set<Person>()
                    .Where(p => p.User != null && userIds.Contains(p.User.Id))
                    .Include(p => p.User)
                    .AsNoTracking()
                    .ToListAsync();

                // Cargar RoleUsers
                var roleUsers = await _context.Set<RoleUser>()
                    .Where(ru => userIds.Contains(ru.UserId))
                    .Include(ru => ru.Role)
                    .AsNoTracking()
                    .ToListAsync();

                // Cargar UserNotifications
                var userNotifications = await _context.Set<UserNotification>()
                    .Where(un => userIds.Contains(un.UserId))
                    .AsNoTracking()
                    .ToListAsync();

                // Cargar PaymentHistories
                var paymentHistories = await _context.Set<PaymentHistory>()
                    .Where(ph => userIds.Contains(ph.UserId))
                    .AsNoTracking()
                    .ToListAsync();

                // Asociar las propiedades cargadas con los usuarios
                foreach (var user in users)
                {
                    user.Person = personsWithUsers.FirstOrDefault(p => p.User?.Id == user.Id)?.User?.Person;
                    user.RoleUsers = roleUsers.Where(ru => ru.UserId == user.Id).ToList();
                    user.UserNotifications = userNotifications.Where(un => un.UserId == user.Id).ToList();
                    user.PaymentHistories = paymentHistories.Where(ph => ph.UserId == user.Id).ToList();
                }

                return users;
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
                // Primero obtenemos el usuario básico
                var user = await _context.Set<User>()
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user != null)
                {
                    // Asignamos valores predeterminados a campos NULL
                    if (user.Username == null)
                    {
                        user.Username = $"usuario-{user.Id}";
                    }
                    if (user.Email == null)
                    {
                        user.Email = $"email-{user.Id}@ejemplo.com";
                    }
                    if (user.Password == null)
                    {
                        user.Password = "password-placeholder";
                    }

                    // Cargar manualmente las propiedades de navegación
                    await _context.Entry(user)
                        .Reference(u => u.Person)
                        .LoadAsync();

                    await _context.Entry(user)
                        .Collection(u => u.RoleUsers)
                        .LoadAsync();

                    await _context.Entry(user)
                        .Collection(u => u.UserNotifications)
                        .LoadAsync();

                    await _context.Entry(user)
                        .Collection(u => u.PaymentHistories)
                        .LoadAsync();
                }

                return user;
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
                // Asegurarse de que no haya valores NULL en campos obligatorios
                if (string.IsNullOrEmpty(user.Username))
                {
                    user.Username = $"usuario-{DateTime.Now.Ticks}";
                }
                if (string.IsNullOrEmpty(user.Email))
                {
                    user.Email = $"email-{DateTime.Now.Ticks}@ejemplo.com";
                }
                if (string.IsNullOrEmpty(user.Password))
                {
                    user.Password = "password-placeholder";
                }

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

                // Asegurarse de que no haya valores NULL en campos obligatorios
                if (string.IsNullOrEmpty(user.Username))
                {
                    user.Username = $"usuario-{user.Id}";
                }
                if (string.IsNullOrEmpty(user.Email))
                {
                    user.Email = $"email-{user.Id}@ejemplo.com";
                }
                if (string.IsNullOrEmpty(user.Password))
                {
                    // Mantener la contraseña existente si no se proporciona una nueva
                    user.Password = existingUser.Password;
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
        /// Verifica si existe una persona con el ID proporcionado.
        /// </summary>
        /// <param name="personId">ID de la persona a verificar.</param>
        /// <returns>True si la persona existe, False en caso contrario.</returns>
        public async Task<bool> PersonExistsAsync(int personId)
        {
            return await _context.Person.AnyAsync(p => p.Id == personId);
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