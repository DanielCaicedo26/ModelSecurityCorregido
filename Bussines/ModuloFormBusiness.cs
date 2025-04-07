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
                return MapToDTOList(moduloForms);
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
                    FormId = moduloFormDto.FormId
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
                FormId = moduloForm.FormId
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


