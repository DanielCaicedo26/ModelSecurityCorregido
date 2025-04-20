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
    /// Clase que maneja la lógica de negocio para los permisos de formularios de roles.
    /// </summary>
    public class RoleFormPermissionBusiness
    {
        private readonly RoleFormPermissionData _roleFormPermissionData;
        private readonly ILogger<RoleFormPermissionBusiness> _logger;

        /// <summary>
        /// Constructor de la clase RoleFormPermissionBusiness.
        /// </summary>
        /// <param name="roleFormPermissionData">Instancia de RoleFormPermissionData para acceder a los datos de los permisos de formularios de roles.</param>
        /// <param name="logger">Instancia de ILogger para el registro de logs.</param>
        public RoleFormPermissionBusiness(RoleFormPermissionData roleFormPermissionData, ILogger<RoleFormPermissionBusiness> logger)
        {
            _roleFormPermissionData = roleFormPermissionData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los permisos de formularios de roles de manera asíncrona.
        /// </summary>
        /// <returns>Una lista de objetos RoleFormPermissionDto.</returns>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar los permisos de formularios de roles.</exception>
        public async Task<IEnumerable<RoleFormPermissionDto>> GetAllRoleFormPermissionsAsync()
        {
            try
            {
                var roleFormPermissions = await _roleFormPermissionData.GetAllAsync();
                var visibleroleFormPermissions = roleFormPermissions.Where(si => si.IsActive);
                return MapToDTOList(visibleroleFormPermissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los permisos de formularios de roles");
                throw new ExternalServiceException("Base de datos", "Error al recuperar los permisos de formularios de roles", ex);
            }
        }

        /// <summary>
        /// Actualiza los datos de un permiso de formulario de rol.
        /// </summary>
        /// <param name="roleFormPermissionDto">El objeto RoleFormPermissionDto con los datos actualizados del permiso de formulario de rol.</param>
        /// <returns>El objeto RoleFormPermissionDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si los datos son inválidos.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra el permiso de formulario de rol.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar el permiso de formulario de rol.</exception>
        public async Task<RoleFormPermissionDto> UpdateRoleFormPermissionAsync(RoleFormPermissionDto roleFormPermissionDto)
        {
            if (roleFormPermissionDto == null || roleFormPermissionDto.Id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar un permiso de formulario de rol con datos inválidos o ID inválido.");
                throw new ValidationException("id", "El ID debe ser mayor que cero y los datos no pueden ser nulos.");
            }

            // Validar los datos del DTO
            ValidateRoleFormPermission(roleFormPermissionDto);

            try
            {
                // Verificar si el permiso de formulario de rol existe
                var existingRoleFormPermission = await _roleFormPermissionData.GetByIdAsync(roleFormPermissionDto.Id);
                if (existingRoleFormPermission == null)
                {
                    _logger.LogInformation("No se encontró ningún permiso de formulario de rol con ID: {RoleFormPermissionId}", roleFormPermissionDto.Id);
                    throw new EntityNotFoundException("RoleFormPermission", roleFormPermissionDto.Id);
                }

                // Actualizar los datos del permiso de formulario de rol
                existingRoleFormPermission.RoleId = roleFormPermissionDto.RoleId;
                existingRoleFormPermission.FormId = roleFormPermissionDto.FormId;
                existingRoleFormPermission.PermissionId = roleFormPermissionDto.PermissionId;
                existingRoleFormPermission.CanCreate = roleFormPermissionDto.CanCreate;
                existingRoleFormPermission.CanRead = roleFormPermissionDto.CanRead;
                existingRoleFormPermission.CanUpdate = roleFormPermissionDto.CanUpdate;
                existingRoleFormPermission.CanDelete = roleFormPermissionDto.CanDelete;

                var isUpdated = await _roleFormPermissionData.UpdateAsync(existingRoleFormPermission);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar el permiso de formulario de rol con ID {roleFormPermissionDto.Id}.");
                }

                return MapToDTO(existingRoleFormPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el permiso de formulario de rol con ID: {RoleFormPermissionId}", roleFormPermissionDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el permiso de formulario de rol con ID {roleFormPermissionDto.Id}.", ex);
            }
        }

        /// <summary>
        /// Elimina un permiso de formulario de rol por su ID de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID del permiso de formulario de rol a eliminar.</param>
        /// <returns>Un objeto RoleFormPermissionDto del permiso de formulario de rol eliminado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el permiso de formulario de rol.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al eliminar el permiso de formulario de rol.</exception>
        public async Task<RoleFormPermissionDto> DeleteRoleFormPermissionAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un permiso de formulario de rol con ID inválido: {RoleFormPermissionId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                // Verificar si el permiso de formulario de rol existe
                var roleFormPermission = await _roleFormPermissionData.GetByIdAsync(id);
                if (roleFormPermission == null)
                {
                    _logger.LogInformation("No se encontró ningún permiso de formulario de rol con ID: {RoleFormPermissionId}", id);
                    throw new EntityNotFoundException("RoleFormPermission", id);
                }

                // Intentar eliminar el permiso de formulario de rol
                var isDeleted = await _roleFormPermissionData.DeleteAsync(id);
                if (!isDeleted)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo eliminar el permiso de formulario de rol con ID {id}");
                }

                // Devolver el objeto eliminado mapeado a DTO
                return MapToDTO(roleFormPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el permiso de formulario de rol con ID: {RoleFormPermissionId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el permiso de formulario de rol con ID {id}", ex);
            }
        }

        /// <summary>
        /// Actualiza los permisos específicos (CanCreate, CanRead, CanUpdate, CanDelete) de un permiso de formulario de rol.
        /// </summary>
        /// <param name="id">El ID del permiso de formulario de rol.</param>
        /// <param name="canCreate">Nuevo valor para CanCreate.</param>
        /// <param name="canRead">Nuevo valor para CanRead.</param>
        /// <param name="canUpdate">Nuevo valor para CanUpdate.</param>
        /// <param name="canDelete">Nuevo valor para CanDelete.</param>
        /// <returns>El objeto RoleFormPermissionDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si los datos son inválidos.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra el permiso de formulario de rol.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar los permisos.</exception>
        public async Task<RoleFormPermissionDto> UpdatePermissionsAsync(int id, bool canCreate, bool canRead, bool canUpdate, bool canDelete)
        {
            if (id <= 0)
            {
                throw new ValidationException("Datos inválidos", "El ID debe ser mayor que cero.");
            }

            try
            {
                var roleFormPermission = await _roleFormPermissionData.GetByIdAsync(id);
                if (roleFormPermission == null)
                {
                    throw new EntityNotFoundException("RoleFormPermission", id);
                }

                // Actualizar las propiedades específicas
                roleFormPermission.CanCreate = canCreate;
                roleFormPermission.CanRead = canRead;
                roleFormPermission.CanUpdate = canUpdate;
                roleFormPermission.CanDelete = canDelete;

                var isUpdated = await _roleFormPermissionData.UpdateAsync(roleFormPermission);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar los permisos del formulario de rol con ID {id}");
                }

                return MapToDTO(roleFormPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar los permisos del formulario de rol con ID: {RoleFormPermissionId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar los permisos del formulario de rol con ID {id}", ex);
            }
        }

        /// <summary>
        /// Activa o desactiva un permiso de formulario de rol por su ID.
        /// </summary>
        /// <param name="id">El ID del permiso de formulario de rol.</param>
        /// <param name="isActive">Estado deseado: true para activar, false para desactivar.</param>
        /// <returns>El objeto RoleFormPermissionDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra el permiso de formulario de rol.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar el estado.</exception>
        public async Task<RoleFormPermissionDto> SetActiveStatusAsync(int id, bool isActive)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó cambiar el estado de un permiso de formulario de rol con ID inválido: {RoleFormPermissionId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var roleFormPermission = await _roleFormPermissionData.GetByIdAsync(id);
                if (roleFormPermission == null)
                {
                    throw new EntityNotFoundException("RoleFormPermission", id);
                }

                // Actualizar el estado activo
                roleFormPermission.IsActive = isActive;

                var isUpdated = await _roleFormPermissionData.UpdateAsync(roleFormPermission);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo cambiar el estado del permiso de formulario de rol con ID {id}");
                }

                return MapToDTO(roleFormPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar el estado del permiso de formulario de rol con ID: {RoleFormPermissionId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al cambiar el estado del permiso de formulario de rol con ID {id}", ex);
            }
        }





        /// <summary>
        /// Obtiene un permiso de formulario de rol por su ID de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID del permiso de formulario de rol.</param>
        /// <returns>Un objeto RoleFormPermissionDto.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el permiso de formulario de rol.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar el permiso de formulario de rol.</exception>
        public async Task<RoleFormPermissionDto> GetRoleFormPermissionByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un permiso de formulario de rol con ID inválido: {RoleFormPermissionId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var roleFormPermission = await _roleFormPermissionData.GetByIdAsync(id);
                if (roleFormPermission == null)
                {
                    _logger.LogInformation("No se encontró ningún permiso de formulario de rol con ID: {RoleFormPermissionId}", id);
                    throw new EntityNotFoundException("RoleFormPermission", id);
                }

                return MapToDTO(roleFormPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el permiso de formulario de rol con ID: {RoleFormPermissionId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el permiso de formulario de rol con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo permiso de formulario de rol de manera asíncrona.
        /// </summary>
        /// <param name="roleFormPermissionDto">El objeto RoleFormPermissionDto con los datos del permiso de formulario de rol.</param>
        /// <returns>El objeto RoleFormPermissionDto creado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando los datos del permiso de formulario de rol son inválidos.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al crear el permiso de formulario de rol.</exception>
        public async Task<RoleFormPermissionDto> CreateRoleFormPermissionAsync(RoleFormPermissionDto roleFormPermissionDto)
        {
            try
            {
                ValidateRoleFormPermission(roleFormPermissionDto);

                var roleFormPermission = new RoleFormPermission
                {
                    RoleId = roleFormPermissionDto.RoleId,
                    FormId = roleFormPermissionDto.FormId,
                    PermissionId = roleFormPermissionDto.PermissionId,
                    CanCreate = roleFormPermissionDto.CanCreate,
                    CanRead = roleFormPermissionDto.CanRead,
                    CanUpdate = roleFormPermissionDto.CanUpdate,
                    CanDelete = roleFormPermissionDto.CanDelete,
                    IsActive = roleFormPermissionDto.IsActive
                };

                var createdRoleFormPermission = await _roleFormPermissionData.CreateAsync(roleFormPermission);
                return MapToDTO(createdRoleFormPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo permiso de formulario de rol");
                throw new ExternalServiceException("Base de datos", "Error al crear el permiso de formulario de rol", ex);
            }
        }

        /// <summary>
        /// Valida los datos del permiso de formulario de rol.
        /// </summary>
        /// <param name="roleFormPermissionDto">El objeto RoleFormPermissionDto con los datos del permiso de formulario de rol.</param>
        /// <exception cref="ValidationException">Lanzada cuando los datos del permiso de formulario de rol son inválidos.</exception>
        private void ValidateRoleFormPermission(RoleFormPermissionDto roleFormPermissionDto)
        {
            if (roleFormPermissionDto == null)
            {
                throw new ValidationException("El objeto RoleFormPermission no puede ser nulo");
            }

            if (roleFormPermissionDto.RoleId <= 0)
            {
                _logger.LogWarning("Se intentó crear un permiso de formulario de rol con RoleId inválido");
                throw new ValidationException("RoleId", "El RoleId debe ser mayor que cero");
            }

            if (roleFormPermissionDto.FormId <= 0)
            {
                _logger.LogWarning("Se intentó crear un permiso de formulario de rol con FormId inválido");
                throw new ValidationException("FormId", "El FormId debe ser mayor que cero");
            }

            if (roleFormPermissionDto.PermissionId <= 0)
            {
                _logger.LogWarning("Se intentó crear un permiso de formulario de rol con PermissionId inválido");
                throw new ValidationException("PermissionId", "El PermissionId debe ser mayor que cero");
            }
        }

        /// <summary>
        /// Mapea un objeto RoleFormPermission a RoleFormPermissionDto.
        /// </summary>
        /// <param name="roleFormPermission">El objeto RoleFormPermission a mapear.</param>
        /// <returns>El objeto RoleFormPermissionDto mapeado.</returns>
        private static RoleFormPermissionDto MapToDTO(RoleFormPermission roleFormPermission)
        {
            return new RoleFormPermissionDto
            {
                Id = roleFormPermission.Id,
                RoleId = roleFormPermission.RoleId,
                FormId = roleFormPermission.FormId,
                PermissionId = roleFormPermission.PermissionId,
                CanCreate = roleFormPermission.CanCreate,
                CanRead = roleFormPermission.CanRead,
                CanUpdate = roleFormPermission.CanUpdate,
                CanDelete = roleFormPermission.CanDelete,
                IsActive = roleFormPermission.IsActive
            };
        }

        /// <summary>
        /// Mapea una lista de objetos RoleFormPermission a una lista de RoleFormPermissionDto.
        /// </summary>
        /// <param name="roleFormPermissions">La lista de objetos RoleFormPermission a mapear.</param>
        /// <returns>La lista de objetos RoleFormPermissionDto mapeados.</returns>
        private static IEnumerable<RoleFormPermissionDto> MapToDTOList(IEnumerable<RoleFormPermission> roleFormPermissions)
        {
            var roleFormPermissionsDto = new List<RoleFormPermissionDto>();
            foreach (var roleFormPermission in roleFormPermissions)
            {
                roleFormPermissionsDto.Add(MapToDTO(roleFormPermission));
            }
            return roleFormPermissionsDto;
        }
    }
}





