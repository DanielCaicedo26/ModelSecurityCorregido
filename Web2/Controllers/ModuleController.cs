using Bussines;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ModuleController : ControllerBase
    {
        private readonly ModuleBusiness _moduleBusiness;
        private readonly ILogger<ModuleController> _logger;

        public ModuleController(ModuleBusiness moduleBusiness, ILogger<ModuleController> logger)
        {
            _moduleBusiness = moduleBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los m�dulos.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ModuleDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var modules = await _moduleBusiness.GetAllModulesAsync();
                return Ok(modules);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener los m�dulos");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un m�dulo por ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ModuleDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var module = await _moduleBusiness.GetModuleByIdAsync(id);
                return Ok(module);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validaci�n fallida para el ID del m�dulo: {ModuleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "M�dulo no encontrado con ID: {ModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el m�dulo con ID: {ModuleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo m�dulo.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ModuleDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create([FromBody] ModuleDto moduleDto)
        {
            try
            {
                var created = await _moduleBusiness.CreateModuleAsync(moduleDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validaci�n fallida al crear el m�dulo");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el m�dulo");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un m�dulo por su ID.
        /// </summary>
        /// <param name="id">El ID del m�dulo a eliminar.</param>
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
                var deletedModule = await _moduleBusiness.DeleteModuleAsync(id);
                return Ok(new { message = "M�dulo eliminado correctamente", data = deletedModule }); // 200 OK
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validaci�n fallida al eliminar el m�dulo con ID: {ModuleId}", id);
                return BadRequest(new { message = ex.Message }); // 400 Bad Request
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "M�dulo no encontrado con ID: {ModuleId}", id);
                return NotFound(new { message = ex.Message }); // 404 Not Found
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el m�dulo con ID: {ModuleId}", id);
                return StatusCode(500, new { message = ex.Message }); // 500 Internal Server Error
            }
        }

        /// <summary>
        /// Actualiza un m�dulo existente.
        /// </summary>
        /// <param name="id">El ID del m�dulo a actualizar.</param>
        /// <param name="moduleDto">El objeto ModuleDto con los datos actualizados.</param>
        /// <returns>Un resultado indicando el �xito o fallo de la operaci�n.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> Update(int id, [FromBody] ModuleDto moduleDto)
        {
            if (id != moduleDto.Id)
            {
                return BadRequest(new { message = "El ID en la URL no coincide con el ID en el cuerpo de la solicitud" });
            }

            try
            {
                var updatedModule = await _moduleBusiness.UpdateModuleAsync(moduleDto);
                return Ok(new { message = "M�dulo actualizado correctamente", data = updatedModule }); // 200 OK
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validaci�n fallida al actualizar el m�dulo con ID: {ModuleId}", id);
                return BadRequest(new { message = ex.Message }); // 400 Bad Request
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "M�dulo no encontrado con ID: {ModuleId}", id);
                return NotFound(new { message = ex.Message }); // 404 Not Found
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el m�dulo con ID: {ModuleId}", id);
                return StatusCode(500, new { message = ex.Message }); // 500 Internal Server Error
            }
        }

        [HttpPatch("{id}/Update-Name-Description-Statu")]
        [ProducesResponseType(typeof(ModuloFormDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Updatee(int id, [FromBody] ModuleDto dto)
        {
            try
            {
                if (id != dto.Id)
                {
                    return BadRequest(new { message = "El ID en la URL no coincide con el ID en el cuerpo." });
                }

                var updated = await _moduleBusiness.Update(dto.Id, dto.Name, dto.Description, dto.Statu);
                return Ok(updated);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validaci�n fallida");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Notificaci�n no encontrada con ID: {UserNotificationId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar notificaci�n con ID: {UserNotificationId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Activa o desactiva una notificaci�n de usuario por ID.
        /// </summary>
        /// <param name="id">El ID de la notificaci�n de usuario.</param>
        /// <param name="isActive">Estado deseado: true para activar, false para desactivar.</param>
        /// <returns>Un c�digo de estado indicando el resultado.</returns>
        [HttpPatch("{id}/active")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SetActiveStatus(int id, [FromBody] bool isActive)
        {
            try
            {
                var updatedNotification = await _moduleBusiness.SetActiveStatusAsync(id, isActive);
                return Ok(updatedNotification);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validaci�n fallida para el ID de la notificaci�n de usuario: {UserNotificationId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Notificaci�n de usuario no encontrada con ID: {UserNotificationId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al cambiar el estado de la notificaci�n de usuario con ID: {UserNotificationId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }


    }
}

