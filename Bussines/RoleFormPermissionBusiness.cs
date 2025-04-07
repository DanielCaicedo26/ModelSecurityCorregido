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
                return MapToDTOList(roleFormPermissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los permisos de formularios de roles");
                throw new ExternalServiceException("Base de datos", "Error al recuperar los permisos de formularios de roles", ex);
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





