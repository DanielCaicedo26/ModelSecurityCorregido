using Data;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Bussines
{
    /// <summary>
    /// Clase que maneja la l�gica de negocio para los formularios de m�dulos.
    /// </summary>
    public class ModuloFormBusiness
    {
        private readonly ModuloFormData _moduloFormData;
        private readonly ILogger<ModuloFormBusiness> _logger;

        /// <summary>
        /// Constructor de la clase ModuloFormBusiness.
        /// </summary>
        /// <param name="moduloFormData">Instancia de ModuloFormData para acceder a los datos de los formularios de m�dulos.</param>
        /// <param name="logger">Instancia de ILogger para el registro de logs.</param>
        public ModuloFormBusiness(ModuloFormData moduloFormData, ILogger<ModuloFormBusiness> logger)
        {
            _moduloFormData = moduloFormData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los formularios de m�dulos de manera as�ncrona.
        /// </summary>
        /// <returns>Una lista de objetos ModuloFormDto.</returns>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar los formularios de m�dulos.</exception>
        public async Task<IEnumerable<ModuloFormDto>> GetAllModuloFormsAsync()
        {
            try
            {
                var moduloForms = await _moduloFormData.GetAllAsync();
                var visibelmoduleform = moduloForms.Where(m => m.IsActive);
                return MapToDTOList(visibelmoduleform);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los formularios de m�dulos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar los formularios de m�dulos", ex);
            }
        }

        /// <summary>
        /// Obtiene un formulario de m�dulo por su ID de manera as�ncrona.
        /// </summary>
        /// <param name="id">El ID del formulario de m�dulo.</param>
        /// <returns>Un objeto ModuloFormDto.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inv�lido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el formulario de m�dulo.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar el formulario de m�dulo.</exception>
        public async Task<ModuloFormDto> GetModuloFormByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� obtener un formulario de m�dulo con ID inv�lido: {ModuloFormId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var moduloForm = await _moduloFormData.GetByIdAsync(id);
                if (moduloForm == null)
                {
                    _logger.LogInformation("No se encontr� ning�n formulario de m�dulo con ID: {ModuloFormId}", id);
                    throw new EntityNotFoundException("ModuloForm", id);
                }

                return MapToDTO(moduloForm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el formulario de m�dulo con ID: {ModuloFormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el formulario de m�dulo con ID {id}", ex);
            }
        }

        /// <summary>
        /// Elimina un formulario de m�dulo por su ID de manera as�ncrona.
        /// </summary>
        /// <param name="id">El ID del formulario de m�dulo a eliminar.</param>
        /// <returns>Un objeto ModuloFormDto del formulario de m�dulo eliminado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inv�lido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el formulario de m�dulo.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al eliminar el formulario de m�dulo.</exception>
        public async Task<ModuloFormDto> DeleteModuloFormAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� eliminar un formulario de m�dulo con ID inv�lido: {ModuloFormId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                // Verificar si el formulario de m�dulo existe
                var moduloForm = await _moduloFormData.GetByIdAsync(id);
                if (moduloForm == null)
                {
                    _logger.LogInformation("No se encontr� ning�n formulario de m�dulo con ID: {ModuloFormId}", id);
                    throw new EntityNotFoundException("ModuloForm", id);
                }

                // Intentar eliminar el formulario de m�dulo
                var isDeleted = await _moduloFormData.DeleteAsync(id);
                if (!isDeleted)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo eliminar el formulario de m�dulo con ID {id}");
                }

                // Devolver el objeto eliminado mapeado a DTO
                return MapToDTO(moduloForm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el formulario de m�dulo con ID: {ModuloFormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el formulario de m�dulo con ID {id}", ex);
            }
        }

        public async Task<ModuloFormDto> Update(int id, int formId, int moduleId)
        {
            if (id <= 0)
            {
                throw new ValidationException("Datos inv�lidos", "ID debe ser mayor que cero y el mensaje no puede estar vac�o");
            }

            try
            {
                var userNotification = await _moduloFormData.GetByIdAsync(id);
                if (userNotification == null)
                {
                    throw new EntityNotFoundException("UserNotification", id);
                }

                userNotification.FormId = formId;
                userNotification.ModuleId = moduleId;

                var isUpdated = await _moduloFormData.UpdateAsync(userNotification);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar la notificaci�n con ID {id}");
                }

                return MapToDTO(userNotification);
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
        public async Task<ModuloFormDto> SetActiveStatusAsync(int id, bool isActive)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� cambiar el estado de un historial de pago con ID inv�lido.");
                throw new ValidationException("id", "El ID debe ser mayor que cero.");
            }

            try
            {
                var moduloForm = await _moduloFormData.GetByIdAsync(id);
                if (moduloForm == null)
                {
                    throw new EntityNotFoundException("moduloForm", id);
                }

                // Actualizar el estado activo
                moduloForm.IsActive = isActive;

                var isUpdated = await _moduloFormData.UpdateAsync(moduloForm);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo cambiar el estado del historial de pago con ID {id}.");
                }

                return MapToDTO( moduloForm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar el estado del historial de pago con ID: {PaymentHistoryId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al cambiar el estado del historial de pago con ID {id}.", ex);
            }
        }


        /// <summary>
        /// Actualiza un formulario de m�dulo existente de manera as�ncrona.
        /// </summary>
        /// <param name="moduloFormDto">El objeto ModuloFormDto con los datos actualizados del formulario de m�dulo.</param>
        /// <returns>El objeto ModuloFormDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si los datos son inv�lidos.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra el formulario de m�dulo.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar el formulario de m�dulo.</exception>
        public async Task<ModuloFormDto> UpdateModuloFormAsync(ModuloFormDto moduloFormDto)
        {
            if (moduloFormDto == null || moduloFormDto.Id <= 0)
            {
                _logger.LogWarning("Se intent� actualizar un formulario de m�dulo con datos inv�lidos o ID inv�lido.");
                throw new ValidationException("id", "El ID debe ser mayor que cero y los datos no pueden ser nulos.");
            }

            // Validar los datos del DTO
            ValidateModuloForm(moduloFormDto);

            try
            {
                // Verificar si el formulario de m�dulo existe
                var existingModuloForm = await _moduloFormData.GetByIdAsync(moduloFormDto.Id);
                if (existingModuloForm == null)
                {
                    _logger.LogInformation("No se encontr� ning�n formulario de m�dulo con ID: {ModuloFormId}", moduloFormDto.Id);
                    throw new EntityNotFoundException("ModuloForm", moduloFormDto.Id);
                }

                // Actualizar los datos del formulario de m�dulo
                existingModuloForm.FormId = moduloFormDto.FormId;
                existingModuloForm.ModuleId = moduloFormDto.ModuleId;

                var isUpdated = await _moduloFormData.UpdateAsync(existingModuloForm);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar el formulario de m�dulo con ID {moduloFormDto.Id}.");
                }

                return MapToDTO(existingModuloForm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el formulario de m�dulo con ID: {ModuloFormId}", moduloFormDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el formulario de m�dulo con ID {moduloFormDto.Id}.", ex);
            }
        }




        /// <summary>
        /// Crea un nuevo formulario de m�dulo de manera as�ncrona.
        /// </summary>
        /// <param name="moduloFormDto">El objeto ModuloFormDto con los datos del formulario de m�dulo.</param>
        /// <returns>El objeto ModuloFormDto creado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando los datos del formulario de m�dulo son inv�lidos.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al crear el formulario de m�dulo.</exception>
        public async Task<ModuloFormDto> CreateModuloFormAsync(ModuloFormDto moduloFormDto)
        {
            try
            {
                ValidateModuloForm(moduloFormDto);

                var moduloForm = new ModuloForm
                {
                    FormId = moduloFormDto.FormId,
                    ModuleId = moduloFormDto.ModuleId,
                    IsActive = moduloFormDto.IsActive
                };

                var createdModuloForm = await _moduloFormData.CreateAsync(moduloForm);
                return MapToDTO(createdModuloForm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo formulario de m�dulo");
                throw new ExternalServiceException("Base de datos", "Error al crear el formulario de m�dulo", ex);
            }
        }

        /// <summary>
        /// Valida los datos del formulario de m�dulo.
        /// </summary>
        /// <param name="moduloFormDto">El objeto ModuloFormDto con los datos del formulario de m�dulo.</param>
        /// <exception cref="ValidationException">Lanzada cuando los datos del formulario de m�dulo son inv�lidos.</exception>
        private void ValidateModuloForm(ModuloFormDto moduloFormDto)
        {
            if (moduloFormDto == null)
            {
                throw new ValidationException("El objeto ModuloForm no puede ser nulo");
            }

            if (moduloFormDto.FormId <= 0)
            {
                _logger.LogWarning("Se intent� crear un formulario de m�dulo con FormId inv�lido");
                throw new ValidationException("FormId", "El FormId debe ser mayor que cero");
            }
        }

        /// <summary>
        /// Mapea un objeto ModuloForm a ModuloFormDto.
        /// </summary>
        /// <param name="moduloForm">El objeto ModuloForm a mapear.</param>
        /// <returns>El objeto ModuloFormDto mapeado.</returns>
        private static ModuloFormDto MapToDTO(ModuloForm moduloForm)
        {
            return new ModuloFormDto
            {
                Id = moduloForm.Id,
                FormId = moduloForm.FormId,
                ModuleId = moduloForm.ModuleId,
                IsActive = moduloForm.IsActive
            };
        }

        /// <summary>
        /// Mapea una lista de objetos ModuloForm a una lista de ModuloFormDto.
        /// </summary>
        /// <param name="moduloForms">La lista de objetos ModuloForm a mapear.</param>
        /// <returns>La lista de objetos ModuloFormDto mapeados.</returns>
        private static IEnumerable<ModuloFormDto> MapToDTOList(IEnumerable<ModuloForm> moduloForms)
        {
            var moduloFormsDto = new List<ModuloFormDto>();
            foreach (var moduloForm in moduloForms)
            {
                moduloFormsDto.Add(MapToDTO(moduloForm));
            }
            return moduloFormsDto;
        }
    }
}


