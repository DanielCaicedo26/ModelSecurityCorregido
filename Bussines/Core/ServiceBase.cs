using Business.Core;
using Data.Core;
using Mapster;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Bussines.Core
{
    public abstract class ServiceBase<TDto, TEntity> : IServiceBase<TDto, TEntity>
        where TEntity : class
    {
        protected readonly IRepository<TEntity> _repository;
        protected readonly ILogger _logger;

        protected ServiceBase(IRepository<TEntity> repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public virtual async Task<IEnumerable<TDto>> GetAllAsync()
        {
            try
            {
                var entities = await _repository.GetAllAsync();
                return entities.Adapt<IEnumerable<TDto>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los registros de {Entity}", typeof(TEntity).Name);
                throw new ExternalServiceException("Repository", ex.Message, ex);
            }
        }

        public virtual async Task<TDto> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ValidationException("Id", "El ID debe ser mayor que cero");
                }

                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                {
                    throw new EntityNotFoundException(typeof(TEntity).Name, id);
                }

                return entity.Adapt<TDto>();
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el registro con ID {Id} de {Entity}", id, typeof(TEntity).Name);
                throw new ExternalServiceException("Repository", ex.Message, ex);
            }
        }

        public virtual async Task<TDto> CreateAsync(TDto dto)
        {
            try
            {
                // Validar DTO (implementar en clases derivadas si es necesario)
                ValidateDto(dto);

                var entity = dto.Adapt<TEntity>();
                var createdEntity = await _repository.AddAsync(entity);
                return createdEntity.Adapt<TDto>();
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear entidad {Entity}", typeof(TEntity).Name);
                throw new ExternalServiceException("Repository", ex.Message, ex);
            }
        }

        public virtual async Task<TDto> UpdateAsync(TDto dto)
        {
            try
            {
                // Validar DTO (implementar en clases derivadas si es necesario)
                ValidateDto(dto);

                var entity = dto.Adapt<TEntity>();
                var updated = await _repository.UpdateAsync(entity);
                if (!updated)
                {
                    throw new EntityNotFoundException(typeof(TEntity).Name, GetEntityId(dto));
                }

                return dto;
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
                _logger.LogError(ex, "Error al actualizar entidad {Entity}", typeof(TEntity).Name);
                throw new ExternalServiceException("Repository", ex.Message, ex);
            }
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ValidationException("Id", "El ID debe ser mayor que cero");
                }

                var success = await _repository.DeleteAsync(id);
                if (!success)
                {
                    throw new EntityNotFoundException(typeof(TEntity).Name, id);
                }

                return true;
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
                _logger.LogError(ex, "Error al eliminar el registro con ID {Id} de {Entity}", id, typeof(TEntity).Name);
                throw new ExternalServiceException("Repository", ex.Message, ex);
            }
        }

        public virtual async Task<TDto> SetActiveStatusAsync(int id, bool isActive)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ValidationException("Id", "El ID debe ser mayor que cero");
                }

                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                {
                    throw new EntityNotFoundException(typeof(TEntity).Name, id);
                }

                // Actualizar estado activo
                var propIsActive = entity.GetType().GetProperty("IsActive");
                if (propIsActive != null)
                {
                    propIsActive.SetValue(entity, isActive);
                    await _repository.UpdateAsync(entity);
                }
                else
                {
                    throw new BusinessRuleViolationException("IsActiveNotFound",
                        $"La entidad {typeof(TEntity).Name} no tiene propiedad IsActive");
                }

                return entity.Adapt<TDto>();
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (BusinessRuleViolationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado activo del registro con ID {Id} de {Entity}", id, typeof(TEntity).Name);
                throw new ExternalServiceException("Repository", ex.Message, ex);
            }
        }

        // Métodos auxiliares para implementar en clases derivadas
        protected virtual void ValidateDto(TDto dto)
        {
            // Por defecto no hace nada, sobrescribir en clases derivadas
        }

        protected virtual int GetEntityId(TDto dto)
        {
            var idProperty = typeof(TDto).GetProperty("Id");
            if (idProperty == null)
            {
                throw new BusinessRuleViolationException("IdNotFound",
                    $"La entidad {typeof(TDto).Name} no tiene propiedad Id");
            }

            return (int)idProperty.GetValue(dto);
        }
    }
}