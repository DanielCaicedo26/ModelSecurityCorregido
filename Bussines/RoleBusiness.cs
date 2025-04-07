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
    /// Clase que maneja la lógica de negocio para los roles.
    /// </summary>
    public class RoleBusiness
    {
        private readonly RoleData _roleData;
        private readonly ILogger<RoleBusiness> _logger;

        /// <summary>
        /// Constructor de la clase RoleBusiness.
        /// </summary>
        /// <param name="roleData">Instancia de RoleData para acceder a los datos de los roles.</param>
        /// <param name="logger">Instancia de ILogger para el registro de logs.</param>
        public RoleBusiness(RoleData roleData, ILogger<RoleBusiness> logger)
        {
            _roleData = roleData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los roles de manera asíncrona.
        /// </summary>
        /// <returns>Una lista de objetos RoleDto.</returns>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar los roles.</exception>
        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            try
            {
                var roles = await _roleData.GetAllAsync();
                return MapToDTOList(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los roles");
                throw new ExternalServiceException("Base de datos", "Error al recuperar los roles", ex);
            }
        }

        /// <summary>
        /// Obtiene un rol por su ID de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID del rol.</param>
        /// <returns>Un objeto RoleDto.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el rol.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar el rol.</exception>
        public async Task<RoleDto> GetRoleByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un rol con ID inválido: {RoleId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var role = await _roleData.GetByIdAsync(id);
                if (role == null)
                {
                    _logger.LogInformation("No se encontró ningún rol con ID: {RoleId}", id);
                    throw new EntityNotFoundException("Role", id);
                }

                return MapToDTO(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol con ID: {RoleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rol con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo rol de manera asíncrona.
        /// </summary>
        /// <param name="roleDto">El objeto RoleDto con los datos del rol.</param>
        /// <returns>El objeto RoleDto creado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando los datos del rol son inválidos.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al crear el rol.</exception>
        public async Task<RoleDto> CreateRoleAsync(RoleDto roleDto)
        {
            try
            {
                ValidateRole(roleDto);

                var role = new Role
                {
                    RoleName = roleDto.RoleName,
                    CreatedAt = DateTime.UtcNow
                };

                var createdRole = await _roleData.CreateAsync(role);
                return MapToDTO(createdRole);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo rol");
                throw new ExternalServiceException("Base de datos", "Error al crear el rol", ex);
            }
        }

        /// <summary>
        /// Valida los datos del rol.
        /// </summary>
        /// <param name="roleDto">El objeto RoleDto con los datos del rol.</param>
        /// <exception cref="ValidationException">Lanzada cuando los datos del rol son inválidos.</exception>
        private void ValidateRole(RoleDto roleDto)
        {
            if (roleDto == null)
            {
                throw new ValidationException("El objeto Role no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(roleDto.RoleName))
            {
                _logger.LogWarning("Se intentó crear un rol con nombre vacío");
                throw new ValidationException("RoleName", "El nombre del rol no puede estar vacío");
            }
        }

        /// <summary>
        /// Mapea un objeto Role a RoleDto.
        /// </summary>
        /// <param name="role">El objeto Role a mapear.</param>
        /// <returns>El objeto RoleDto mapeado.</returns>
        private static RoleDto MapToDTO(Role role)
        {
            return new RoleDto
            {
                Id = role.Id,
                RoleName = role.RoleName
            };
        }

        /// <summary>
        /// Mapea una lista de objetos Role a una lista de RoleDto.
        /// </summary>
        /// <param name="roles">La lista de objetos Role a mapear.</param>
        /// <returns>La lista de objetos RoleDto mapeados.</returns>
        private static IEnumerable<RoleDto> MapToDTOList(IEnumerable<Role> roles)
        {
            var rolesDto = new List<RoleDto>();
            foreach (var role in roles)
            {
                rolesDto.Add(MapToDTO(role));
            }
            return rolesDto;
        }
    }
}





