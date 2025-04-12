
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Bussines
{
    /// <summary>
    /// Clase que maneja la lógica de negocio para los usuarios.
    /// </summary>
    public class UserBusiness
    {
        private readonly UserData _userData;
        private readonly ILogger<UserBusiness> _logger;

        /// <summary>
        /// Constructor de la clase UserBusiness.
        /// </summary>
        /// <param name="userData">Instancia de UserData para acceder a los datos de los usuarios.</param>
        /// <param name="logger">Instancia de ILogger para el registro de logs.</param>
        public UserBusiness(UserData userData, ILogger<UserBusiness> logger)
        {
            _userData = userData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los usuarios de manera asíncrona.
        /// </summary>
        /// <returns>Una lista de objetos UserDto.</returns>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar los usuarios.</exception>
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userData.GetAllAsync();
                return MapToDTOList(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios");
                throw new ExternalServiceException("Base de datos", "Error al recuperar los usuarios", ex);
            }
        }

        /// <summary>
        /// Obtiene un usuario por su ID de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID del usuario.</param>
        /// <returns>Un objeto UserDto.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el usuario.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar el usuario.</exception>
        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un usuario con ID inválido: {UserId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var user = await _userData.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogInformation("No se encontró ningún usuario con ID: {UserId}", id);
                    throw new EntityNotFoundException("User", id);
                }

                return MapToDTO(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con ID: {UserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el usuario con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo usuario de manera asíncrona.
        /// </summary>
        /// <param name="userDto">El objeto UserDto con los datos del usuario.</param>
        /// <returns>El objeto UserDto creado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando los datos del usuario son inválidos.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al crear el usuario.</exception>
        public async Task<UserDto> CreateUserAsync(UserDto userDto)
        {
            try
            {
                ValidateUser(userDto);

                // Verificar si el PersonId existe en la tabla Person
                var personExists = await _userData.PersonExistsAsync(userDto.PersonId);
                if (!personExists)
                {
                    _logger.LogWarning("El PersonId {PersonId} no existe en la base de datos", userDto.PersonId);
                    throw new ValidationException("PersonId", "El PersonId proporcionado no existe");
                }

                var user = new User
                {
                    Username = userDto.Username,
                    Email = userDto.Email,
                    Password = userDto.Password,
                    CreatedAt = DateTime.UtcNow,
                    PersonId = userDto.PersonId // Asegúrate de que esta propiedad esté asignada
                };

                var createdUser = await _userData.CreateAsync(user);
                return MapToDTO(createdUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo usuario");
                throw new ExternalServiceException("Base de datos", "Error al crear el usuario", ex);
            }
        }


        /// <summary>
        /// Valida los datos del usuario.
        /// </summary>
        /// <param name="userDto">El objeto UserDto con los datos del usuario.</param>
        /// <exception cref="ValidationException">Lanzada cuando los datos del usuario son inválidos.</exception>
        private void ValidateUser(UserDto userDto)
        {
            if (userDto == null)
            {
                throw new ValidationException("El objeto User no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(userDto.Username))
            {
                _logger.LogWarning("Se intentó crear un usuario con nombre de usuario vacío");
                throw new ValidationException("Username", "El nombre de usuario no puede estar vacío");
            }

            if (string.IsNullOrWhiteSpace(userDto.Email))
            {
                _logger.LogWarning("Se intentó crear un usuario con correo electrónico vacío");
                throw new ValidationException("Email", "El correo electrónico no puede estar vacío");
            }

            if (string.IsNullOrWhiteSpace(userDto.Password))
            {
                _logger.LogWarning("Se intentó crear un usuario con contraseña vacía");
                throw new ValidationException("Password", "La contraseña no puede estar vacía");
            }
        }

        /// <summary>
        /// Mapea un objeto User a UserDto.
        /// </summary>
        /// <param name="user">El objeto User a mapear.</param>
        /// <returns>El objeto UserDto mapeado.</returns>
        private static UserDto MapToDTO(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Password = user.Password
            };
        }

        /// <summary>
        /// Mapea una lista de objetos User a una lista de UserDto.
        /// </summary>
        /// <param name="users">La lista de objetos User a mapear.</param>
        /// <returns>La lista de objetos UserDto mapeados.</returns>
        private static IEnumerable<UserDto> MapToDTOList(IEnumerable<User> users)
        {
            var usersDto = new List<UserDto>();
            foreach (var user in users)
            {
                usersDto.Add(MapToDTO(user));
            }
            return usersDto;
        }
    }
}










