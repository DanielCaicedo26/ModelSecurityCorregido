using Bussines.Core;
using Bussines.interfaces;
using Data.Interfaces;
using Entity.Dto;
using Entity.Model;
using Mapster;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Bussines.Services
{
    public class RoleBusiness : ServiceBase<RoleDto, Role>, IRoleBusiness
    {
        private readonly IRoleRepository _roleRepository;

        public RoleBusiness(IRoleRepository repository, ILogger<RoleBusiness> logger)
            : base(repository, logger)
        {
            _roleRepository = repository;
        }

        /// <summary>
        /// Implementación explícita del método Update de IServiceBase
        /// </summary>
        async Task<RoleDto> IServiceBase<RoleDto, Role>.Update(int id, RoleDto dto)
        {
            return await base.Update(id, dto);
        }

        /// <summary>
        /// Obtiene un rol por su nombre.
        /// </summary>
        public async Task<RoleDto?> GetByNameAsync(string roleName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    _logger.LogWarning("Se intentó obtener un rol con nombre vacío");
                    throw new ValidationException("roleName", "El nombre del rol no puede estar vacío");
                }

                var role = await _roleRepository.GetByNameAsync(roleName);
                if (role == null)
                {
                    _logger.LogInformation("No se encontró ningún rol con nombre: {RoleName}", roleName);
                    return null;
                }

                return role.Adapt<RoleDto>();
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol con nombre: {RoleName}", roleName);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rol con nombre {roleName}", ex);
            }
        }

        /// <summary>
        /// Obtiene todos los roles asociados a un usuario específico.
        /// </summary>
        public async Task<IEnumerable<RoleDto>> GetByUserIdAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    _logger.LogWarning("Se intentó obtener roles con un ID de usuario inválido: {UserId}", userId);
                    throw new ValidationException("userId", "El ID de usuario debe ser mayor que cero");
                }

                var roles = await _roleRepository.GetByUserIdAsync(userId);
                return roles.Adapt<IEnumerable<RoleDto>>();
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los roles del usuario con ID: {UserId}", userId);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar los roles del usuario con ID {userId}", ex);
            }
        }

        /// <summary>
        /// Actualiza el nombre y la descripción de un rol.
        /// </summary>
        public async Task<RoleDto> Update(int id, string roleName, string? description)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Se intentó actualizar un rol con ID inválido: {RoleId}", id);
                    throw new ValidationException("id", "El ID debe ser mayor que cero");
                }

                if (string.IsNullOrWhiteSpace(roleName))
                {
                    _logger.LogWarning("Se intentó actualizar un rol con nombre vacío");
                    throw new ValidationException("roleName", "El nombre del rol no puede estar vacío");
                }

                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogInformation("No se encontró ningún rol con ID: {RoleId}", id);
                    throw new EntityNotFoundException("Role", id);
                }

                entity.RoleName = roleName;
                entity.Description = description;

                var isUpdated = await _repository.UpdateAsync(entity);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar el rol con ID {id}");
                }

                return entity.Adapt<RoleDto>();
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el rol con ID: {RoleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el rol con ID {id}", ex);
            }
        }

        /// <summary>
        /// Valida los datos del DTO de rol.
        /// </summary>
        protected override void ValidateDto(RoleDto dto)
        {
            if (dto == null)
            {
                throw new ValidationException("El objeto Role no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(dto.RoleName))
            {
                _logger.LogWarning("Se intentó crear un rol con nombre vacío");
                throw new ValidationException("RoleName", "El nombre del rol no puede estar vacío");
            }
        }
    }
}