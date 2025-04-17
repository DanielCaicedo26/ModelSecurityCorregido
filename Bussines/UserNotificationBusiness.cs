using System;
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
    /// Clase que maneja la lógica de negocio para las notificaciones de usuario.
    /// </summary>
    public class UserNotificationBusiness
    {
        private readonly UserNotificationData _userNotificationData;
        private readonly ILogger<UserNotificationBusiness> _logger;

        /// <summary>
        /// Constructor de la clase UserNotificationBusiness.
        /// </summary>
        /// <param name="userNotificationData">Instancia de UserNotificationData para acceder a los datos de las notificaciones de usuario.</param>
        /// <param name="logger">Instancia de ILogger para el registro de logs.</param>
        public UserNotificationBusiness(UserNotificationData userNotificationData, ILogger<UserNotificationBusiness> logger)
        {
            _userNotificationData = userNotificationData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las notificaciones de usuario de manera asíncrona.
        /// </summary>
        /// <returns>Una lista de objetos UserNotificationDto.</returns>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar las notificaciones de usuario.</exception>
        public async Task<IEnumerable<UserNotificationDto>> GetAllUserNotificationsAsync()
        {
            try
            {
                var userNotifications = await _userNotificationData.GetAllAsync();
                var visibleNotifications = userNotifications.Where(n => !n.IsHidden); // Excluir ocultos
                return MapToDTOList(visibleNotifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las notificaciones de usuario");
                throw new ExternalServiceException("Base de datos", "Error al recuperar las notificaciones de usuario", ex);
            }
        }

        /// <summary>
        /// Obtiene una notificación de usuario por su ID de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID de la notificación de usuario.</param>
        /// <returns>Un objeto UserNotificationDto.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra la notificación de usuario.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar la notificación de usuario.</exception>
        public async Task<UserNotificationDto> GetUserNotificationByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una notificación de usuario con ID inválido: {UserNotificationId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var userNotification = await _userNotificationData.GetByIdAsync(id);
                if (userNotification == null)
                {
                    _logger.LogInformation("No se encontró ninguna notificación de usuario con ID: {UserNotificationId}", id);
                    throw new EntityNotFoundException("UserNotification", id);
                }

                return MapToDTO(userNotification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la notificación de usuario con ID: {UserNotificationId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la notificación de usuario con ID {id}", ex);
            }
        }

        /// <summary>
        /// Elimina una notificación de usuario por su ID de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID de la notificación de usuario a eliminar.</param>
        /// <returns>Un objeto UserNotificationDto de la notificación eliminada.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra la notificación de usuario.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al eliminar la notificación de usuario.</exception>
        public async Task<UserNotificationDto> DeleteUserNotificationAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar una notificación de usuario con ID inválido: {UserNotificationId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var userNotification = await _userNotificationData.GetByIdAsync(id);
                if (userNotification == null)
                {
                    _logger.LogInformation("No se encontró ninguna notificación de usuario con ID: {UserNotificationId}", id);
                    throw new EntityNotFoundException("UserNotification", id);
                }

                var isDeleted = await _userNotificationData.DeleteAsync(id);
                if (!isDeleted)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo eliminar la notificación de usuario con ID {id}");
                }

                return MapToDTO(userNotification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la notificación de usuario con ID: {UserNotificationId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar la notificación de usuario con ID {id}", ex);
            }
        }

        /// <summary>
        /// Actualiza los datos de una notificación de usuario.
        /// </summary>
        /// <param name="userNotificationDto">El objeto con los datos actualizados de la notificación.</param>
        /// <returns>El objeto UserNotificationDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si los datos son inválidos.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra la notificación.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar los datos.</exception>
        public async Task<UserNotificationDto> UpdateUserNotificationAsync(UserNotificationDto userNotificationDto)
        {
            if (userNotificationDto == null || userNotificationDto.Id <= 0)
            {
                throw new ValidationException("id", "El ID debe ser mayor que cero y los datos no pueden ser nulos.");
            }

            try
            {
                var userNotification = await _userNotificationData.GetByIdAsync(userNotificationDto.Id);
                if (userNotification == null)
                {
                    throw new EntityNotFoundException("UserNotification", userNotificationDto.Id);
                }

                userNotification.Message = userNotificationDto.Message;
                userNotification.IsRead = userNotificationDto.IsRead;

                var isUpdated = await _userNotificationData.UpdateAsync(userNotification);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar la notificación con ID {userNotificationDto.Id}");
                }

                return MapToDTO(userNotification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la notificación de usuario con ID: {UserNotificationId}", userNotificationDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la notificación con ID {userNotificationDto.Id}", ex);
            }
        }

        public async Task<UserNotificationDto> UpdateNotificationVisibilityAsync(int id, bool isHidden)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar la visibilidad de una notificación con ID inválido: {UserNotificationId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var userNotification = await _userNotificationData.GetByIdAsync(id);
                if (userNotification == null)
                {
                    throw new EntityNotFoundException("UserNotification", id);
                }

                userNotification.IsHidden = isHidden;
                var isUpdated = await _userNotificationData.UpdateAsync(userNotification);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar la visibilidad de la notificación con ID {id}");
                }

                return MapToDTO(userNotification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la visibilidad de la notificación de usuario con ID: {UserNotificationId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la visibilidad de la notificación con ID {id}", ex);
            }
        }



        /// <summary>
        /// Crea una nueva notificación de usuario de manera asíncrona.
        /// </summary>
        /// <param name="userNotificationDto">El objeto UserNotificationDto con los datos de la notificación de usuario.</param>
        /// <returns>El objeto UserNotificationDto creado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando los datos de la notificación de usuario son inválidos.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al crear la notificación de usuario.</exception>
        public async Task<UserNotificationDto> CreateUserNotificationAsync(UserNotificationDto userNotificationDto)
        {
            try
            {
                ValidateUserNotification(userNotificationDto);

                var userNotification = new UserNotification
                {
                    UserId = userNotificationDto.UserId,
                    Message = userNotificationDto.Message,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                var createdUserNotification = await _userNotificationData.CreateAsync(userNotification);
                return MapToDTO(createdUserNotification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear una nueva notificación de usuario");
                throw new ExternalServiceException("Base de datos", "Error al crear la notificación de usuario", ex);
            }
        }

        /// <summary>
        /// Valida los datos de la notificación de usuario.
        /// </summary>
        /// <param name="userNotificationDto">El objeto UserNotificationDto con los datos de la notificación de usuario.</param>
        /// <exception cref="ValidationException">Lanzada cuando los datos de la notificación de usuario son inválidos.</exception>
        private void ValidateUserNotification(UserNotificationDto userNotificationDto)
        {
            if (userNotificationDto == null)
            {
                throw new ValidationException("El objeto UserNotification no puede ser nulo");
            }

            if (userNotificationDto.UserId <= 0)
            {
                _logger.LogWarning("Se intentó crear una notificación de usuario con UserId inválido");
                throw new ValidationException("UserId", "El UserId debe ser mayor que cero");
            }

            if (string.IsNullOrWhiteSpace(userNotificationDto.Message))
            {
                _logger.LogWarning("Se intentó crear una notificación de usuario con mensaje vacío");
                throw new ValidationException("Message", "El mensaje no puede estar vacío");
            }
        }

        /// <summary>
        /// Mapea un objeto UserNotification a UserNotificationDto.
        /// </summary>
        /// <param name="userNotification">El objeto UserNotification a mapear.</param>
        /// <returns>El objeto UserNotificationDto mapeado.</returns>
        private static UserNotificationDto MapToDTO(UserNotification userNotification)
        {
            return new UserNotificationDto
            {
                Id = userNotification.Id,
                UserId = userNotification.UserId,
                Message = userNotification.Message
            };
        }

        /// <summary>
        /// Mapea una lista de objetos UserNotification a una lista de UserNotificationDto.
        /// </summary>
        /// <param name="userNotifications">La lista de objetos UserNotification a mapear.</param>
        /// <returns>La lista de objetos UserNotificationDto mapeados.</returns>
        private static IEnumerable<UserNotificationDto> MapToDTOList(IEnumerable<UserNotification> userNotifications)
        {
            var userNotificationsDto = new List<UserNotificationDto>();
            foreach (var userNotification in userNotifications)
            {
                userNotificationsDto.Add(MapToDTO(userNotification));
            }
            return userNotificationsDto;
        }
    }
}










