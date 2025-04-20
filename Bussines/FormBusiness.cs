using Data;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Bussines
{
    /// <summary>
    /// Clase que maneja la lógica de negocio para los formularios.
    /// </summary>
    public class FormBusiness
    {
        private readonly FormData _formData;
        private readonly ILogger<FormBusiness> _logger;

        /// <summary>
        /// Constructor de la clase FormBusiness.
        /// </summary>
        /// <param name="formData">Instancia de FormData para acceder a los datos de los formularios.</param>
        /// <param name="logger">Instancia de ILogger para el registro de logs.</param>
        public FormBusiness(FormData formData, ILogger<FormBusiness> logger)
        {
            _formData = formData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los formularios de manera asíncrona.
        /// </summary>
        /// <returns>Una lista de objetos FormDto.</returns>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar los formularios.</exception>
        public async Task<IEnumerable<FormDto>> GetAllFormsAsync()
        {
            try
            {
                var forms = await _formData.GetAllAsync();
                var Visiblesforms = forms.Where(n => n.IsActive);
                return MapToDTOList(Visiblesforms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los formularios");
                throw new ExternalServiceException("Base de datos", "Error al recuperar los formularios", ex);
            }
        }

        /// <summary>
        /// Obtiene un formulario por su ID de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID del formulario.</param>
        /// <returns>Un objeto FormDto.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el formulario.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar el formulario.</exception>
        public async Task<FormDto> GetFormByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un formulario con ID inválido: {FormId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var form = await _formData.GetByIdAsync(id);
                if (form == null)
                {
                    _logger.LogInformation("No se encontró ningún formulario con ID: {FormId}", id);
                    throw new EntityNotFoundException("Form", id);
                }

                return MapToDTO(form);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el formulario con ID: {FormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el formulario con ID {id}", ex);
            }
        }

        /// <summary>
        /// Elimina un formulario por su ID de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID del formulario a eliminar.</param>
        /// <returns>Un objeto FormDto del formulario eliminado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el formulario.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al eliminar el formulario.</exception>
        public async Task<FormDto> DeleteFormAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un formulario con ID inválido: {FormId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                // Verificar si el formulario existe
                var form = await _formData.GetByIdAsync(id);
                if (form == null)
                {
                    _logger.LogInformation("No se encontró ningún formulario con ID: {FormId}", id);
                    throw new EntityNotFoundException("Form", id);
                }

                // Intentar eliminar el formulario
                var isDeleted = await _formData.DeleteAsync(id);
                if (!isDeleted)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo eliminar el formulario con ID {id}");
                }

                // Devolver el objeto eliminado mapeado a DTO
                return MapToDTO(form);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el formulario con ID: {FormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el formulario con ID {id}", ex);
            }
        }

        public async Task<FormDto> Update(int id, string name, string? description, string? status)
        {
            if (id <= 0 || string.IsNullOrWhiteSpace(name))
            {
                throw new ValidationException("Datos inválidos", "El ID debe ser mayor que cero y el nombre no puede estar vacío.");
            }

            try
            {
                var form = await _formData.GetByIdAsync(id);
                if (form == null)
                {
                    throw new EntityNotFoundException("Form", id);
                }

                form.Name = name;
                form.Description = description;
                form.Status = status;

                var isUpdated = await _formData.UpdateAsync(form);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar el formulario con ID {id}");
                }

                return MapToDTO(form);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el formulario con ID: {FormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el formulario con ID {id}", ex);
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
        public async Task<FormDto> SetActiveStatusAsync(int id, bool isActive)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó cambiar el estado de un historial de pago con ID inválido.");
                throw new ValidationException("id", "El ID debe ser mayor que cero.");
            }

            try
            {
                var Form = await _formData.GetByIdAsync(id);
                if (Form == null)
                {
                    throw new EntityNotFoundException("module", id);
                }

                // Actualizar el estado activo
                Form.IsActive = isActive;

                var isUpdated = await _formData.UpdateAsync(Form);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo cambiar el estado del historial de pago con ID {id}.");
                }

                return MapToDTO(Form);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar el estado del historial de pago con ID: {PaymentHistoryId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al cambiar el estado del historial de pago con ID {id}.", ex);
            }
        }


        /// <summary>
        /// Actualiza un formulario existente de manera asíncrona.
        /// </summary>
        /// <param name="formDto">El objeto FormDto con los datos actualizados del formulario.</param>
        /// <returns>El objeto FormDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si los datos son inválidos.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra el formulario.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar el formulario.</exception>
        public async Task<FormDto> UpdateFormAsync(FormDto formDto)
        {
            if (formDto == null || formDto.Id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar un formulario con datos inválidos o ID inválido.");
                throw new ValidationException("id", "El ID debe ser mayor que cero y los datos no pueden ser nulos.");
            }

            // Validar los datos del DTO
            ValidateForm(formDto);

            try
            {
                // Verificar si el formulario existe
                var existingForm = await _formData.GetByIdAsync(formDto.Id);
                if (existingForm == null)
                {
                    _logger.LogInformation("No se encontró ningún formulario con ID: {FormId}", formDto.Id);
                    throw new EntityNotFoundException("Form", formDto.Id);
                }

                // Actualizar los datos del formulario
                existingForm.Name = formDto.Name;
                existingForm.Description = formDto.Description;
                existingForm.Status = formDto.Status;

                var isUpdated = await _formData.UpdateAsync(existingForm);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar el formulario con ID {formDto.Id}.");
                }

                return MapToDTO(existingForm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el formulario con ID: {FormId}", formDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el formulario con ID {formDto.Id}.", ex);
            }
        }





        /// <summary>
        /// Crea un nuevo formulario de manera asíncrona.
        /// </summary>
        /// <param name="formDto">El objeto FormDto con los datos del formulario.</param>
        /// <returns>El objeto FormDto creado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando los datos del formulario son inválidos.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al crear el formulario.</exception>
        public async Task<FormDto> CreateFormAsync(FormDto formDto)
        {
            try
            {
                ValidateForm(formDto);

                var form = new Form
                {
                    Name = formDto.Name,
                    Description = formDto.Description,
                    DateCreation = DateTime.UtcNow,
                    Status = formDto.Status,
                    IsActive = formDto.IsActive
                };

                var createdForm = await _formData.CreateAsync(form);
                return MapToDTO(createdForm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo formulario");
                throw new ExternalServiceException("Base de datos", "Error al crear el formulario", ex);
            }
        }

        /// <summary>
        /// Valida los datos del formulario.
        /// </summary>
        /// <param name="formDto">El objeto FormDto con los datos del formulario.</param>
        /// <exception cref="ValidationException">Lanzada cuando los datos del formulario son inválidos.</exception>
        private void ValidateForm(FormDto formDto)
        {
            if (formDto == null)
            {
                throw new ValidationException("El objeto Form no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(formDto.Name))
            {
                _logger.LogWarning("Se intentó crear un formulario con nombre vacío");
                throw new ValidationException("Name", "El nombre no puede estar vacío");
            }
        }

        /// <summary>
        /// Mapea un objeto Form a FormDto.
        /// </summary>
        /// <param name="form">El objeto Form a mapear.</param>
        /// <returns>El objeto FormDto mapeado.</returns>
        private static FormDto MapToDTO(Form form)
        {
            return new FormDto
            {
                Id = form.Id,
                Name = form.Name,
                Description = form.Description,
                DateCreation = form.DateCreation,
                Status = form.Status,
                IsActive = form.IsActive
            };
        }

        /// <summary>
        /// Mapea una lista de objetos Form a una lista de FormDto.
        /// </summary>
        /// <param name="forms">La lista de objetos Form a mapear.</param>
        /// <returns>La lista de objetos FormDto mapeados.</returns>
        private static IEnumerable<FormDto> MapToDTOList(IEnumerable<Form> forms)
        {
            var formsDto = new List<FormDto>();
            foreach (var form in forms)
            {
                formsDto.Add(MapToDTO(form));
            }
            return formsDto;
        }
    }
}

