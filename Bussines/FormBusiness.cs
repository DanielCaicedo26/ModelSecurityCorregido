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
                return MapToDTOList(forms);
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
                    Status = formDto.Status
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
                Status = form.Status
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

