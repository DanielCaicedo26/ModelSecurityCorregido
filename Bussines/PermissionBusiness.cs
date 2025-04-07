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
    /// Clase que maneja la l�gica de negocio para los permisos.
    /// </summary>
    public class PermissionBusiness
    {
        private readonly PermissionData _permissionData;
        private readonly ILogger<PermissionBusiness> _logger;

        /// <summary>
        /// Constructor de la clase PermissionBusiness.
        /// </summary>
        /// <param name="permissionData">Instancia de PermissionData para acceder a los datos de los permisos.</param>
        /// <param name="logger">Instancia de ILogger para el registro de logs.</param>
        public PermissionBusiness(PermissionData permissionData, ILogger<PermissionBusiness> logger)
        {
            _permissionData = permissionData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los permisos de manera as�ncrona.
        /// </summary>
        /// <returns>Una lista de objetos PermissionDto.</returns>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar los permisos.</exception>
        public async Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync()
        {
            try
            {
                var permissions = await _permissionData.GetAllAsync();
                return MapToDTOList(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los permisos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar los permisos", ex);
            }
        }

        /// <summary>
        /// Obtiene un permiso por su ID de manera as�ncrona.
        /// </summary>
        /// <param name="id">El ID del permiso.</param>
        /// <returns>Un objeto PermissionDto.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inv�lido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el permiso.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar el permiso.</exception>
        public async Task<PermissionDto> GetPermissionByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� obtener un permiso con ID inv�lido: {PermissionId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var permission = await _permissionData.GetByIdAsync(id);
                if (permission == null)
                {
                    _logger.LogInformation("No se encontr� ning�n permiso con ID: {PermissionId}", id);
                    throw new EntityNotFoundException("Permission", id);
                }

                return MapToDTO(permission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el permiso con ID: {PermissionId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el permiso con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo permiso de manera as�ncrona.
        /// </summary>
        /// <param name="permissionDto">El objeto PermissionDto con los datos del permiso.</param>
        /// <returns>El objeto PermissionDto creado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando los datos del permiso son inv�lidos.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al crear el permiso.</exception>
        public async Task<PermissionDto> CreatePermissionAsync(PermissionDto permissionDto)
        {
            try
            {
                ValidatePermission(permissionDto);

                var permission = new Permission
                {
                    Name = permissionDto.Name,
                    Description = permissionDto.Description,
                    CreatedAt = DateTime.UtcNow
                };

                var createdPermission = await _permissionData.CreateAsync(permission);
                return MapToDTO(createdPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo permiso");
                throw new ExternalServiceException("Base de datos", "Error al crear el permiso", ex);
            }
        }

        /// <summary>
        /// Valida los datos del permiso.
        /// </summary>
        /// <param name="permissionDto">El objeto PermissionDto con los datos del permiso.</param>
        /// <exception cref="ValidationException">Lanzada cuando los datos del permiso son inv�lidos.</exception>
        private void ValidatePermission(PermissionDto permissionDto)
        {
            if (permissionDto == null)
            {
                throw new ValidationException("El objeto Permission no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(permissionDto.Name))
            {
                _logger.LogWarning("Se intent� crear un permiso con nombre vac�o");
                throw new ValidationException("Name", "El nombre no puede estar vac�o");
            }
        }

        /// <summary>
        /// Mapea un objeto Permission a PermissionDto.
        /// </summary>
        /// <param name="permission">El objeto Permission a mapear.</param>
        /// <returns>El objeto PermissionDto mapeado.</returns>
        private static PermissionDto MapToDTO(Permission permission)
        {
            return new PermissionDto
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.Description
            };
        }

        /// <summary>
        /// Mapea una lista de objetos Permission a una lista de PermissionDto.
        /// </summary>
        /// <param name="permissions">La lista de objetos Permission a mapear.</param>
        /// <returns>La lista de objetos PermissionDto mapeados.</returns>
        private static IEnumerable<PermissionDto> MapToDTOList(IEnumerable<Permission> permissions)
        {
            var permissionsDto = new List<PermissionDto>();
            foreach (var permission in permissions)
            {
                permissionsDto.Add(MapToDTO(permission));
            }
            return permissionsDto;
        }
    }
}




