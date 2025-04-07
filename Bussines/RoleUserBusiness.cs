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
                return MapToDTOList(roleUsers);
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
                    CreatedAt = DateTime.UtcNow
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
                UserId = roleUser.UserId
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






