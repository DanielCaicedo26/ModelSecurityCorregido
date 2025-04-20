using Data;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Bussines
{
    /// <summary>
    /// Clase que maneja la l�gica de negocio para los m�dulos.
    /// </summary>
    public class ModuleBusiness
    {
        private readonly ModuleData _moduleData;
        private readonly ILogger<ModuleBusiness> _logger;

        /// <summary>
        /// Constructor de la clase ModuleBusiness.
        /// </summary>
        /// <param name="moduleData">Instancia de ModuleData para acceder a los datos de los m�dulos.</param>
        /// <param name="logger">Instancia de ILogger para el registro de logs.</param>
        public ModuleBusiness(ModuleData moduleData, ILogger<ModuleBusiness> logger)
        {
            _moduleData = moduleData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los m�dulos de manera as�ncrona.
        /// </summary>
        /// <returns>Una lista de objetos ModuleDto.</returns>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar los m�dulos.</exception>
        public async Task<IEnumerable<ModuleDto>> GetAllModulesAsync()
        {
            try
            {
                var modules = await _moduleData.GetAllAsync();
                var visiblesmodules = modules.Where(n =>n.IsActive);
                return MapToDTOList(visiblesmodules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los m�dulos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar los m�dulos", ex);
            }
        }

        /// <summary>
        /// Obtiene un m�dulo por su ID de manera as�ncrona.
        /// </summary>
        /// <param name="id">El ID del m�dulo.</param>
        /// <returns>Un objeto ModuleDto.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inv�lido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el m�dulo.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar el m�dulo.</exception>
        public async Task<ModuleDto> GetModuleByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� obtener un m�dulo con ID inv�lido: {ModuleId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var module = await _moduleData.GetByIdAsync(id);
                if (module == null)
                {
                    _logger.LogInformation("No se encontr� ning�n m�dulo con ID: {ModuleId}", id);
                    throw new EntityNotFoundException("Module", id);
                }

                return MapToDTO(module);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el m�dulo con ID: {ModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el m�dulo con ID {id}", ex);
            }
        }

        /// <summary>
        /// Elimina un m�dulo por su ID de manera as�ncrona.
        /// </summary>
        /// <param name="id">El ID del m�dulo a eliminar.</param>
        /// <returns>Un objeto ModuleDto del m�dulo eliminado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inv�lido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el m�dulo.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al eliminar el m�dulo.</exception>
        public async Task<ModuleDto> DeleteModuleAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� eliminar un m�dulo con ID inv�lido: {ModuleId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                // Verificar si el m�dulo existe
                var module = await _moduleData.GetByIdAsync(id);
                if (module == null)
                {
                    _logger.LogInformation("No se encontr� ning�n m�dulo con ID: {ModuleId}", id);
                    throw new EntityNotFoundException("Module", id);
                }

                // Intentar eliminar el m�dulo
                var isDeleted = await _moduleData.DeleteAsync(id);
                if (!isDeleted)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo eliminar el m�dulo con ID {id}");
                }

                // Devolver el objeto eliminado mapeado a DTO
                return MapToDTO(module);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el m�dulo con ID: {ModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el m�dulo con ID {id}", ex);
            }
        }

        public async Task<ModuleDto> Update(int id, string name, string description, string status)
        {
            if (id <= 0)
            {
                throw new ValidationException("Datos inv�lidos", "ID debe ser mayor que cero y el mensaje no puede estar vac�o");
            }

            try
            {
                var Module = await _moduleData.GetByIdAsync(id);
                if (Module == null)
                {
                    throw new EntityNotFoundException("Module", id);
                }

                Module.Name = name;
                Module.Description = description;
                Module.Statu = status;

                var isUpdated = await _moduleData.UpdateAsync(Module);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar la notificaci�n con ID {id}");
                }

                return MapToDTO(Module);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la notificaci�n con ID: {UserNotificationId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la notificaci�n con ID {id}", ex);
            }
        }

        /// <summary>
        /// Activa o desactiva un historial de pago por su ID.
        /// </summary>
        /// <param name="id">El ID del historial de pago.</param>
        /// <param name="isActive">Estado deseado: true para activar, false para desactivar.</param>
        /// <returns>El objeto PaymentHistoryDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si el ID es inv�lido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra el historial de pago.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar el estado.</exception>
        public async Task<ModuleDto> SetActiveStatusAsync(int id, bool isActive)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� cambiar el estado de un historial de pago con ID inv�lido.");
                throw new ValidationException("id", "El ID debe ser mayor que cero.");
            }

            try
            {
                var module = await _moduleData.GetByIdAsync(id);
                if (module == null)
                {
                    throw new EntityNotFoundException("module", id);
                }

                // Actualizar el estado activo
                module.IsActive = isActive;

                var isUpdated = await _moduleData.UpdateAsync(module);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo cambiar el estado del historial de pago con ID {id}.");
                }

                return MapToDTO(module);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar el estado del historial de pago con ID: {PaymentHistoryId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al cambiar el estado del historial de pago con ID {id}.", ex);
            }
        }

        /// <summary>
        /// Actualiza un m�dulo existente de manera as�ncrona.
        /// </summary>
        /// <param name="moduleDto">El objeto ModuleDto con los datos actualizados del m�dulo.</param>
        /// <returns>El objeto ModuleDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si los datos son inv�lidos.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra el m�dulo.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar el m�dulo.</exception>
        public async Task<ModuleDto> UpdateModuleAsync(ModuleDto moduleDto)
        {
            if (moduleDto == null || moduleDto.Id <= 0)
            {
                _logger.LogWarning("Se intent� actualizar un m�dulo con datos inv�lidos o ID inv�lido.");
                throw new ValidationException("id", "El ID debe ser mayor que cero y los datos no pueden ser nulos.");
            }

            // Validar los datos del DTO
            ValidateModule(moduleDto);

            try
            {
                // Verificar si el m�dulo existe
                var existingModule = await _moduleData.GetByIdAsync(moduleDto.Id);
                if (existingModule == null)
                {
                    _logger.LogInformation("No se encontr� ning�n m�dulo con ID: {ModuleId}", moduleDto.Id);
                    throw new EntityNotFoundException("Module", moduleDto.Id);
                }

                // Actualizar los datos del m�dulo
                existingModule.Name = moduleDto.Name;
                existingModule.Description = moduleDto.Description;
                existingModule.Statu = moduleDto.Statu;

                var isUpdated = await _moduleData.UpdateAsync(existingModule);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar el m�dulo con ID {moduleDto.Id}.");
                }

                return MapToDTO(existingModule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el m�dulo con ID: {ModuleId}", moduleDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el m�dulo con ID {moduleDto.Id}.", ex);
            }
        }



        /// <summary>
        /// Crea un nuevo m�dulo de manera as�ncrona.
        /// </summary>
        /// <param name="moduleDto">El objeto ModuleDto con los datos del m�dulo.</param>
        /// <returns>El objeto ModuleDto creado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando los datos del m�dulo son inv�lidos.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al crear el m�dulo.</exception>
        public async Task<ModuleDto> CreateModuleAsync(ModuleDto moduleDto)
        {
            try
            {
                ValidateModule(moduleDto);

                var module = new Module
                {
                    Name = moduleDto.Name,
                    Description = moduleDto.Description,
                    Statu = moduleDto.Statu,
                    IsActive = moduleDto.IsActive
                };

                var createdModule = await _moduleData.CreateAsync(module);
                return MapToDTO(createdModule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo m�dulo");
                throw new ExternalServiceException("Base de datos", "Error al crear el m�dulo", ex);
            }
        }

        /// <summary>
        /// Valida los datos del m�dulo.
        /// </summary>
        /// <param name="moduleDto">El objeto ModuleDto con los datos del m�dulo.</param>
        /// <exception cref="ValidationException">Lanzada cuando los datos del m�dulo son inv�lidos.</exception>
        private void ValidateModule(ModuleDto moduleDto)
        {
            if (moduleDto == null)
            {
                throw new ValidationException("El objeto Module no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(moduleDto.Name))
            {
                _logger.LogWarning("Se intent� crear un m�dulo con nombre vac�o");
                throw new ValidationException("Name", "El nombre no puede estar vac�o");
            }
        }

        /// <summary>
        /// Mapea un objeto Module a ModuleDto.
        /// </summary>
        /// <param name="module">El objeto Module a mapear.</param>
        /// <returns>El objeto ModuleDto mapeado.</returns>
        private static ModuleDto MapToDTO(Module module)
        {
            return new ModuleDto
            {
                Id = module.Id,
                Name = module.Name,
                Description = module.Description,
                Statu = module.Statu,
                IsActive = module.IsActive
            };
        }

        /// <summary>
        /// Mapea una lista de objetos Module a una lista de ModuleDto.
        /// </summary>
        /// <param name="modules">La lista de objetos Module a mapear.</param>
        /// <returns>La lista de objetos ModuleDto mapeados.</returns>
        private static IEnumerable<ModuleDto> MapToDTOList(IEnumerable<Module> modules)
        {
            var modulesDto = new List<ModuleDto>();
            foreach (var module in modules)
            {
                modulesDto.Add(MapToDTO(module));
            }
            return modulesDto;
        }
    }
}


