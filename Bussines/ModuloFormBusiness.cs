using Data;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Bussines
{
    /// <summary>
    /// Clase que maneja la lógica de negocio para los formularios de módulos.
    /// </summary>
    public class ModuloFormBusiness
    {
        private readonly ModuloFormData _moduloFormData;
        private readonly ILogger<ModuloFormBusiness> _logger;

        /// <summary>
        /// Constructor de la clase ModuloFormBusiness.
        /// </summary>
        /// <param name="moduloFormData">Instancia de ModuloFormData para acceder a los datos de los formularios de módulos.</param>
        /// <param name="logger">Instancia de ILogger para el registro de logs.</param>
        public ModuloFormBusiness(ModuloFormData moduloFormData, ILogger<ModuloFormBusiness> logger)
        {
            _moduloFormData = moduloFormData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los formularios de módulos de manera asíncrona.
        /// </summary>
        /// <returns>Una lista de objetos ModuloFormDto.</returns>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar los formularios de módulos.</exception>
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
                _logger.LogError(ex, "Error al obtener todos los formularios de módulos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar los formularios de módulos", ex);
            }
        }

        /// <summary>
        /// Obtiene un formulario de módulo por su ID de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID del formulario de módulo.</param>
        /// <returns>Un objeto ModuloFormDto.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el formulario de módulo.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar el formulario de módulo.</exception>
        public async Task<ModuloFormDto> GetModuloFormByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un formulario de módulo con ID inválido: {ModuloFormId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var moduloForm = await _moduloFormData.GetByIdAsync(id);
                if (moduloForm == null)
                {
                    _logger.LogInformation("No se encontró ningún formulario de módulo con ID: {ModuloFormId}", id);
                    throw new EntityNotFoundException("ModuloForm", id);
                }

                return MapToDTO(moduloForm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el formulario de módulo con ID: {ModuloFormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el formulario de módulo con ID {id}", ex);
            }
        }

        /// <summary>
        /// Elimina un formulario de módulo por su ID de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID del formulario de módulo a eliminar.</param>
        /// <returns>Un objeto ModuloFormDto del formulario de módulo eliminado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el formulario de módulo.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al eliminar el formulario de módulo.</exception>
        public async Task<ModuloFormDto> DeleteModuloFormAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un formulario de módulo con ID inválido: {ModuloFormId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                // Verificar si el formulario de módulo existe
                var moduloForm = await _moduloFormData.GetByIdAsync(id);
                if (moduloForm == null)
                {
                    _logger.LogInformation("No se encontró ningún formulario de módulo con ID: {ModuloFormId}", id);
                    throw new EntityNotFoundException("ModuloForm", id);
                }

                // Intentar eliminar el formulario de módulo
                var isDeleted = await _moduloFormData.DeleteAsync(id);
                if (!isDeleted)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo eliminar el formulario de módulo con ID {id}");
                }

                // Devolver el objeto eliminado mapeado a DTO
                return MapToDTO(moduloForm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el formulario de módulo con ID: {ModuloFormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el formulario de módulo con ID {id}", ex);
            }
        }

        public async Task<ModuloFormDto> Update(int id, int formId, int moduleId)
        {
            if (id <= 0)
            {
                throw new ValidationException("Datos inválidos", "ID debe ser mayor que cero y el mensaje no puede estar vacío");
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
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar la notificación con ID {id}");
                }

                return MapToDTO(userNotification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la notificación con ID: {UserNotificationId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la notificación con ID {id}", ex);
            }
        }

        /// <summary>
        /// Activa o desactiva un historial de pago por su ID.
        /// </summary>
        /// <param name="id">El ID del historial de pago.</param>
        /// <param name="isActive">Estado deseado: true para activar, false para desactivar.</param>
        /// <returns>El objeto PaymentHistoryDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra el historial de pago.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar el estado.</exception>
        public async Task<ModuloFormDto> SetActiveStatusAsync(int id, bool isActive)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó cambiar el estado de un historial de pago con ID inválido.");
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
        /// Actualiza un formulario de módulo existente de manera asíncrona.
        /// </summary>
        /// <param name="moduloFormDto">El objeto ModuloFormDto con los datos actualizados del formulario de módulo.</param>
        /// <returns>El objeto ModuloFormDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si los datos son inválidos.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra el formulario de módulo.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar el formulario de módulo.</exception>
        public async Task<ModuloFormDto> UpdateModuloFormAsync(ModuloFormDto moduloFormDto)
        {
            if (moduloFormDto == null || moduloFormDto.Id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar un formulario de módulo con datos inválidos o ID inválido.");
                throw new ValidationException("id", "El ID debe ser mayor que cero y los datos no pueden ser nulos.");
            }

            // Validar los datos del DTO
            ValidateModuloForm(moduloFormDto);

            try
            {
                // Verificar si el formulario de módulo existe
                var existingModuloForm = await _moduloFormData.GetByIdAsync(moduloFormDto.Id);
                if (existingModuloForm == null)
                {
                    _logger.LogInformation("No se encontró ningún formulario de módulo con ID: {ModuloFormId}", moduloFormDto.Id);
                    throw new EntityNotFoundException("ModuloForm", moduloFormDto.Id);
                }

                // Actualizar los datos del formulario de módulo
                existingModuloForm.FormId = moduloFormDto.FormId;
                existingModuloForm.ModuleId = moduloFormDto.ModuleId;

                var isUpdated = await _moduloFormData.UpdateAsync(existingModuloForm);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar el formulario de módulo con ID {moduloFormDto.Id}.");
                }

                return MapToDTO(existingModuloForm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el formulario de módulo con ID: {ModuloFormId}", moduloFormDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el formulario de módulo con ID {moduloFormDto.Id}.", ex);
            }
        }




        /// <summary>
        /// Crea un nuevo formulario de módulo de manera asíncrona.
        /// </summary>
        /// <param name="moduloFormDto">El objeto ModuloFormDto con los datos del formulario de módulo.</param>
        /// <returns>El objeto ModuloFormDto creado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando los datos del formulario de módulo son inválidos.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al crear el formulario de módulo.</exception>
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
                _logger.LogError(ex, "Error al crear un nuevo formulario de módulo");
                throw new ExternalServiceException("Base de datos", "Error al crear el formulario de módulo", ex);
            }
        }

        /// <summary>
        /// Valida los datos del formulario de módulo.
        /// </summary>
        /// <param name="moduloFormDto">El objeto ModuloFormDto con los datos del formulario de módulo.</param>
        /// <exception cref="ValidationException">Lanzada cuando los datos del formulario de módulo son inválidos.</exception>
        private void ValidateModuloForm(ModuloFormDto moduloFormDto)
        {
            if (moduloFormDto == null)
            {
                throw new ValidationException("El objeto ModuloForm no puede ser nulo");
            }

            if (moduloFormDto.FormId <= 0)
            {
                _logger.LogWarning("Se intentó crear un formulario de módulo con FormId inválido");
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


