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
    public class ModuleBusiness : ServiceBase<ModuleDto, Module>, IModuleBusiness
    {
        private readonly IModuleRepository _moduleRepository;

        public ModuleBusiness(IModuleRepository repository, ILogger<ModuleBusiness> logger)
            : base(repository, logger)
        {
            _moduleRepository = repository;
        }

        // Implementación explícita del método Update de IServiceBase
        async Task<ModuleDto> IServiceBase<ModuleDto, Module>.Update(int id, ModuleDto dto)
        {
            return await base.Update(id, dto);
        }

        // Implementación del método Update con 4 parámetros
        public async Task<ModuleDto> Update(int id, string name, string? description, string? status)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Se intentó actualizar un módulo con ID inválido: {ModuleId}", id);
                    throw new ValidationException("id", "El ID debe ser mayor que cero");
                }

                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogInformation("No se encontró ningún módulo con ID: {ModuleId}", id);
                    throw new EntityNotFoundException("Module", id);
                }

                entity.Name = name;
                entity.Description = description;
                entity.Statu = status;

                var isUpdated = await _repository.UpdateAsync(entity);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar el módulo con ID {id}");
                }

                return entity.Adapt<ModuleDto>();
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
                _logger.LogError(ex, "Error al actualizar el módulo con ID: {ModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el módulo con ID {id}", ex);
            }
        }

        // Implementación del método GetByUserIdAsync requerido por IModuleBusiness
        public async Task<IEnumerable<Module>> GetByUserIdAsync(int userId)
        {
            if (userId <= 0)
            {
                _logger.LogWarning("Se intentó obtener módulos con un ID de usuario inválido: {UserId}", userId);
                throw new ValidationException("userId", "El ID de usuario debe ser mayor que cero");
            }

            return await _moduleRepository.GetByUserIdAsync(userId);
        }
    }
}
