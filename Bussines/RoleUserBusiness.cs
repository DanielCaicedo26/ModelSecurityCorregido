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
    /// Clase que maneja la lógica de negocio para los usuarios de roles.
    /// </summary>
    public class RoleUserBusiness
    {
        private readonly RoleUserData _roleUserData;
        private readonly ILogger<RoleUserBusiness> _logger;

        /// <summary>
        /// Constructor de la clase RoleUserBusiness.
        /// </summary>
        /// <param name="roleUserData">Instancia de RoleUserData para acceder a los datos de los usuarios de roles.</param>
        /// <param name="logger">Instancia de ILogger para el registro de logs.</param>
        public RoleUserBusiness(RoleUserData roleUserData, ILogger<RoleUserBusiness> logger)
        {
            _roleUserData = roleUserData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los usuarios de roles de manera asíncrona.
        /// </summary>
        /// <returns>Una lista de objetos RoleUserDto.</returns>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar los usuarios de roles.</exception>
        public async Task<IEnumerable<RoleUserDto>> GetAllRoleUsersAsync()
        {
            try
            {
                var roleUsers = await _roleUserData.GetAllAsync();
                var visibleroleUsers = roleUsers.Where(si => si.IsActive);
                return MapToDTOList(visibleroleUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios de roles");
                throw new ExternalServiceException("Base de datos", "Error al recuperar los usuarios de roles", ex);
            }
        }

        /// <summary>
        /// Obtiene un usuario de rol por su ID de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID del usuario de rol.</param>
        /// <returns>Un objeto RoleUserDto.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el usuario de rol.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar el usuario de rol.</exception>
        public async Task<RoleUserDto> GetRoleUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un usuario de rol con ID inválido: {RoleUserId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var roleUser = await _roleUserData.GetByIdAsync(id);
                if (roleUser == null)
                {
                    _logger.LogInformation("No se encontró ningún usuario de rol con ID: {RoleUserId}", id);
                    throw new EntityNotFoundException("RoleUser", id);
                }

                return MapToDTO(roleUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario de rol con ID: {RoleUserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el usuario de rol con ID {id}", ex);
            }
        }

        /// <summary>
        /// Elimina un usuario de rol por su ID de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID del usuario de rol a eliminar.</param>
        /// <returns>Un objeto RoleUserDto del usuario de rol eliminado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el usuario de rol.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al eliminar el usuario de rol.</exception>
        public async Task<RoleUserDto> DeleteRoleUserAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un usuario de rol con ID inválido: {RoleUserId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                // Verificar si el usuario de rol existe
                var roleUser = await _roleUserData.GetByIdAsync(id);
                if (roleUser == null)
                {
                    _logger.LogInformation("No se encontró ningún usuario de rol con ID: {RoleUserId}", id);
                    throw new EntityNotFoundException("RoleUser", id);
                }

                // Intentar eliminar el usuario de rol
                var isDeleted = await _roleUserData.DeleteAsync(id);
                if (!isDeleted)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo eliminar el usuario de rol con ID {id}");
                }

                // Devolver el objeto eliminado mapeado a DTO
                return MapToDTO(roleUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el usuario de rol con ID: {RoleUserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el usuario de rol con ID {id}", ex);
            }
        }

        /// <summary>
        /// Activa o desactiva un usuario de rol por su ID.
        /// </summary>
        /// <param name="id">El ID del usuario de rol.</param>
        /// <param name="isActive">Estado deseado: true para activar, false para desactivar.</param>
        /// <returns>El objeto RoleUserDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra el usuario de rol.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar el estado.</exception>
        public async Task<RoleUserDto> SetActiveStatusAsync(int id, bool isActive)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó cambiar el estado de un usuario de rol con ID inválido: {RoleUserId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var roleUser = await _roleUserData.GetByIdAsync(id);
                if (roleUser == null)
                {
                    throw new EntityNotFoundException("RoleUser", id);
                }

                // Actualizar el estado activo
                roleUser.IsActive = isActive;

                var isUpdated = await _roleUserData.UpdateAsync(roleUser);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo cambiar el estado del usuario de rol con ID {id}");
                }

                return MapToDTO(roleUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar el estado del usuario de rol con ID: {RoleUserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al cambiar el estado del usuario de rol con ID {id}", ex);
            }
        }

        /// <summary>
        /// Actualiza el RoleId y UserId de un usuario de rol por su ID.
        /// </summary>
        /// <param name="id">El ID del usuario de rol.</param>
        /// <param name="roleId">El nuevo RoleId.</param>
        /// <param name="userId">El nuevo UserId.</param>
        /// <returns>El objeto RoleUserDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si los datos son inválidos.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra el usuario de rol.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar el usuario de rol.</exception>
        public async Task<RoleUserDto> UpdatePartialAsync(int id, int roleId, int userId)
        {
            if (id <= 0 || roleId <= 0 || userId <= 0)
            {
                throw new ValidationException("Datos inválidos", "ID, RoleId y UserId deben ser mayores que cero.");
            }

            try
            {
                var roleUser = await _roleUserData.GetByIdAsync(id);
                if (roleUser == null)
                {
                    throw new EntityNotFoundException("RoleUser", id);
                }

                // Actualizar las propiedades específicas
                roleUser.RoleId = roleId;
                roleUser.UserId = userId;

                var isUpdated = await _roleUserData.UpdateAsync(roleUser);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar el usuario de rol con ID {id}");
                }

                return MapToDTO(roleUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario de rol con ID: {RoleUserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el usuario de rol con ID {id}", ex);
            }
        }



        /// <summary>
        /// Actualiza los datos de un usuario de rol.
        /// </summary>
        /// <param name="roleUserDto">El objeto RoleUserDto con los datos actualizados del usuario de rol.</param>
        /// <returns>El objeto RoleUserDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si los datos son inválidos.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra el usuario de rol.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar el usuario de rol.</exception>
        public async Task<RoleUserDto> UpdateRoleUserAsync(RoleUserDto roleUserDto)
        {
            if (roleUserDto == null || roleUserDto.Id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar un usuario de rol con datos inválidos o ID inválido.");
                throw new ValidationException("id", "El ID debe ser mayor que cero y los datos no pueden ser nulos.");
            }

            // Validar los datos del DTO
            ValidateRoleUser(roleUserDto);

            try
            {
                // Verificar si el usuario de rol existe
                var existingRoleUser = await _roleUserData.GetByIdAsync(roleUserDto.Id);
                if (existingRoleUser == null)
                {
                    _logger.LogInformation("No se encontró ningún usuario de rol con ID: {RoleUserId}", roleUserDto.Id);
                    throw new EntityNotFoundException("RoleUser", roleUserDto.Id);
                }

                // Actualizar los datos del usuario de rol
                existingRoleUser.RoleId = roleUserDto.RoleId;
                existingRoleUser.UserId = roleUserDto.UserId;

                var isUpdated = await _roleUserData.UpdateAsync(existingRoleUser);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar el usuario de rol con ID {roleUserDto.Id}.");
                }

                return MapToDTO(existingRoleUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario de rol con ID: {RoleUserId}", roleUserDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el usuario de rol con ID {roleUserDto.Id}.", ex);
            }
        }



        /// <summary>
        /// Crea un nuevo usuario de rol de manera asíncrona.
        /// </summary>
        /// <param name="roleUserDto">El objeto RoleUserDto con los datos del usuario de rol.</param>
        /// <returns>El objeto RoleUserDto creado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando los datos del usuario de rol son inválidos.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al crear el usuario de rol.</exception>
        public async Task<RoleUserDto> CreateRoleUserAsync(RoleUserDto roleUserDto)
        {
            try
            {
                ValidateRoleUser(roleUserDto);

                var roleUser = new RoleUser
                {
                    RoleId = roleUserDto.RoleId,
                    UserId = roleUserDto.UserId,
                    CreatedAt = DateTime.UtcNow,
                    IsActive=roleUserDto.IsActive
                };

                var createdRoleUser = await _roleUserData.CreateAsync(roleUser);
                return MapToDTO(createdRoleUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo usuario de rol");
                throw new ExternalServiceException("Base de datos", "Error al crear el usuario de rol", ex);
            }
        }

        /// <summary>
        /// Valida los datos del usuario de rol.
        /// </summary>
        /// <param name="roleUserDto">El objeto RoleUserDto con los datos del usuario de rol.</param>
        /// <exception cref="ValidationException">Lanzada cuando los datos del usuario de rol son inválidos.</exception>
        private void ValidateRoleUser(RoleUserDto roleUserDto)
        {
            if (roleUserDto == null)
            {
                throw new ValidationException("El objeto RoleUser no puede ser nulo");
            }

            if (roleUserDto.RoleId <= 0)
            {
                _logger.LogWarning("Se intentó crear un usuario de rol con RoleId inválido");
                throw new ValidationException("RoleId", "El RoleId debe ser mayor que cero");
            }

            if (roleUserDto.UserId <= 0)
            {
                _logger.LogWarning("Se intentó crear un usuario de rol con UserId inválido");
                throw new ValidationException("UserId", "El UserId debe ser mayor que cero");
            }
        }

        /// <summary>
        /// Mapea un objeto RoleUser a RoleUserDto.
        /// </summary>
        /// <param name="roleUser">El objeto RoleUser a mapear.</param>
        /// <returns>El objeto RoleUserDto mapeado.</returns>
        private static RoleUserDto MapToDTO(RoleUser roleUser)
        {
            return new RoleUserDto
            {
                Id = roleUser.Id,
                RoleId = roleUser.RoleId,
                UserId = roleUser.UserId,
                IsActive = roleUser.IsActive
            };
        }

        /// <summary>
        /// Mapea una lista de objetos RoleUser a una lista de RoleUserDto.
        /// </summary>
        /// <param name="roleUsers">La lista de objetos RoleUser a mapear.</param>
        /// <returns>La lista de objetos RoleUserDto mapeados.</returns>
        private static IEnumerable<RoleUserDto> MapToDTOList(IEnumerable<RoleUser> roleUsers)
        {
            var roleUsersDto = new List<RoleUserDto>();
            foreach (var roleUser in roleUsers)
            {
                roleUsersDto.Add(MapToDTO(roleUser));
            }
            return roleUsersDto;
        }
    }
}






