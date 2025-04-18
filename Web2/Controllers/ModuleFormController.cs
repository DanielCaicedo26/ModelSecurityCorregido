using Bussines;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ModuleFormController : ControllerBase
    {
        private readonly ModuloFormBusiness _moduloFormBusiness;
        private readonly ILogger<ModuleFormController> _logger;

        public ModuleFormController(ModuloFormBusiness moduleFormBusiness, ILogger<ModuleFormController> logger)
        {
            _moduloFormBusiness = moduleFormBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los formularios de m�dulos.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ModuloFormDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var moduleForms = await _moduloFormBusiness.GetAllModuloFormsAsync();
                return Ok(moduleForms);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener los formularios de m�dulos");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un formulario de m�dulo por ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ModuloFormDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var moduleForm = await _moduloFormBusiness.GetModuloFormByIdAsync(id);
                return Ok(moduleForm);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validaci�n fallida para el ID del formulario de m�dulo: {ModuleFormId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Formulario de m�dulo no encontrado con ID: {ModuleFormId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el formulario de m�dulo con ID: {ModuleFormId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo formulario de m�dulo.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ModuloFormDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create([FromBody] ModuloFormDto moduleFormDto)
        {
            try
            {
                var created = await _moduloFormBusiness.CreateModuloFormAsync(moduleFormDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validaci�n fallida al crear el formulario de m�dulo");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el formulario de m�dulo");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un formulario de m�dulo por su ID.
        /// </summary>
        /// <param name="id">El ID del formulario de m�dulo a eliminar.</param>
        /// <returns>Un resultado indicando el �xito o fallo de la operaci�n.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deletedModuloForm = await _moduloFormBusiness.DeleteModuloFormAsync(id);
                return Ok(new { message = "Formulario de m�dulo eliminado correctamente", data = deletedModuloForm }); // 200 OK
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validaci�n fallida al eliminar el formulario de m�dulo con ID: {ModuloFormId}", id);
                return BadRequest(new { message = ex.Message }); // 400 Bad Request
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Formulario de m�dulo no encontrado con ID: {ModuloFormId}", id);
                return NotFound(new { message = ex.Message }); // 404 Not Found
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el formulario de m�dulo con ID: {ModuloFormId}", id);
                return StatusCode(500, new { message = ex.Message }); // 500 Internal Server Error
            }
        }

        /// <summary>
        /// Actualiza un formulario de m�dulo existente.
        /// </summary>
        /// <param name="id">El ID del formulario de m�dulo a actualizar.</param>
        /// <param name="moduloFormDto">El objeto ModuloFormDto con los datos actualizados.</param>
        /// <returns>Un resultado indicando el �xito o fallo de la operaci�n.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> Update(int id, [FromBody] ModuloFormDto moduloFormDto)
        {
            if (id != moduloFormDto.Id)
            {
                return BadRequest(new { message = "El ID en la URL no coincide con el ID en el cuerpo de la solicitud" });
            }

            try
            {
                var updatedModuloForm = await _moduloFormBusiness.UpdateModuloFormAsync(moduloFormDto);
                return Ok(new { message = "Formulario de m�dulo actualizado correctamente", data = updatedModuloForm }); // 200 OK
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validaci�n fallida al actualizar el formulario de m�dulo con ID: {ModuloFormId}", id);
                return BadRequest(new { message = ex.Message }); // 400 Bad Request
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Formulario de m�dulo no encontrado con ID: {ModuloFormId}", id);
                return NotFound(new { message = ex.Message }); // 404 Not Found
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el formulario de m�dulo con ID: {ModuloFormId}", id);
                return StatusCode(500, new { message = ex.Message }); // 500 Internal Server Error
            }
        }




    }
}

