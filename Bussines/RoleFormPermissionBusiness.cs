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
    /// Clase que maneja la l�gica de negocio para los permisos de formularios de roles.
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
        /// Obtiene todos los permisos de formularios de roles de manera as�ncrona.
        /// </summary>
        /// <returns>Una lista de objetos RoleFormPermissionDto.</returns>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar los permisos de formularios de roles.</exception>
        public async Task<IEnumerable<RoleFormPermissionDto>> GetAllRoleFormPermissionsAsync()
        {
            try
            {
                var roleFormPermissions = await _roleFormPermissionData.GetAllAsync();
                return MapToDTOList(roleFormPermissions);
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
        /// <exception cref="ValidationException">Lanzada si los datos son inv�lidos.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra el permiso de formulario de rol.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar el permiso de formulario de rol.</exception>
        public async Task<RoleFormPermissionDto> UpdateRoleFormPermissionAsync(RoleFormPermissionDto roleFormPermissionDto)
        {
            if (roleFormPermissionDto == null || roleFormPermissionDto.Id <= 0)
            {
                _logger.LogWarning("Se intent� actualizar un permiso de formulario de rol con datos inv�lidos o ID inv�lido.");
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
                    _logger.LogInformation("No se encontr� ning�n permiso de formulario de rol con ID: {RoleFormPermissionId}", roleFormPermissionDto.Id);
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
        /// Elimina un permiso de formulario de rol por su ID de manera as�ncrona.
        /// </summary>
        /// <param name="id">El ID del permiso de formulario de rol a eliminar.</param>
        /// <returns>Un objeto RoleFormPermissionDto del permiso de formulario de rol eliminado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inv�lido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el permiso de formulario de rol.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al eliminar el permiso de formulario de rol.</exception>
        public async Task<RoleFormPermissionDto> DeleteRoleFormPermissionAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� eliminar un permiso de formulario de rol con ID inv�lido: {RoleFormPermissionId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                // Verificar si el permiso de formulario de rol existe
                var roleFormPermission = await _roleFormPermissionData.GetByIdAsync(id);
                if (roleFormPermission == null)
                {
                    _logger.LogInformation("No se encontr� ning�n permiso de formulario de rol con ID: {RoleFormPermissionId}", id);
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
        /// Obtiene un permiso de formulario de rol por su ID de manera as�ncrona.
        /// </summary>
        /// <param name="id">El ID del permiso de formulario de rol.</param>
        /// <returns>Un objeto RoleFormPermissionDto.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inv�lido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el permiso de formulario de rol.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar el permiso de formulario de rol.</exception>
        public async Task<RoleFormPermissionDto> GetRoleFormPermissionByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� obtener un permiso de formulario de rol con ID inv�lido: {RoleFormPermissionId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var roleFormPermission = await _roleFormPermissionData.GetByIdAsync(id);
                if (roleFormPermission == null)
                {
                    _logger.LogInformation("No se encontr� ning�n permiso de formulario de rol con ID: {RoleFormPermissionId}", id);
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
        /// Crea un nuevo permiso de formulario de rol de manera as�ncrona.
        /// </summary>
        /// <param name="roleFormPermissionDto">El objeto RoleFormPermissionDto con los datos del permiso de formulario de rol.</param>
        /// <returns>El objeto RoleFormPermissionDto creado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando los datos del permiso de formulario de rol son inv�lidos.</exception>
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
                    CanDelete = roleFormPermissionDto.CanDelete
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
        /// <exception cref="ValidationException">Lanzada cuando los datos del permiso de formulario de rol son inv�lidos.</exception>
        private void ValidateRoleFormPermission(RoleFormPermissionDto roleFormPermissionDto)
        {
            if (roleFormPermissionDto == null)
            {
                throw new ValidationException("El objeto RoleFormPermission no puede ser nulo");
            }

            if (roleFormPermissionDto.RoleId <= 0)
            {
                _logger.LogWarning("Se intent� crear un permiso de formulario de rol con RoleId inv�lido");
                throw new ValidationException("RoleId", "El RoleId debe ser mayor que cero");
            }

            if (roleFormPermissionDto.FormId <= 0)
            {
                _logger.LogWarning("Se intent� crear un permiso de formulario de rol con FormId inv�lido");
                throw new ValidationException("FormId", "El FormId debe ser mayor que cero");
            }

            if (roleFormPermissionDto.PermissionId <= 0)
            {
                _logger.LogWarning("Se intent� crear un permiso de formulario de rol con PermissionId inv�lido");
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
                CanDelete = roleFormPermission.CanDelete
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





