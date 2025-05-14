using Bussines.interfaces;
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
        private readonly IModuleBusiness _moduleBusiness;
        private readonly ILogger<ModuleController> _logger;

        public ModuleController(IModuleBusiness moduleBusiness, ILogger<ModuleController> logger)
        {
            _moduleBusiness = moduleBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los módulos.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ModuleDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var modules = await _moduleBusiness.GetAllAsync();
                return Ok(modules);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener los módulos");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un módulo por ID.
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
                var module = await _moduleBusiness.GetByIdAsync(id);
                return Ok(module);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el ID del módulo: {ModuleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Módulo no encontrado con ID: {ModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el módulo con ID: {ModuleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo módulo.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ModuleDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create([FromBody] ModuleDto moduleDto)
        {
            try
            {
                var created = await _moduleBusiness.CreateAsync(moduleDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear el módulo");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el módulo");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un módulo por su ID.
        /// </summary>
        /// <param name="id">El ID del módulo a eliminar.</param>
        /// <returns>Un resultado indicando el éxito o fallo de la operación.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deletedModule = await _moduleBusiness.DeleteAsync(id);
                return Ok(new { message = "Módulo eliminado correctamente", data = deletedModule }); // 200 OK
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar el módulo con ID: {ModuleId}", id);
                return BadRequest(new { message = ex.Message }); // 400 Bad Request
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Módulo no encontrado con ID: {ModuleId}", id);
                return NotFound(new { message = ex.Message }); // 404 Not Found
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el módulo con ID: {ModuleId}", id);
                return StatusCode(500, new { message = ex.Message }); // 500 Internal Server Error
            }
        }

        /// <summary>
        /// Actualiza un módulo existente.
        /// </summary>
        /// <param name="id">El ID del módulo a actualizar.</param>
        /// <param name="moduleDto">El objeto ModuleDto con los datos actualizados.</param>
        /// <returns>Un resultado indicando el éxito o fallo de la operación.</returns>
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
                // Llamamos a Update con id, name, description, statu
                var updatedModule = await _moduleBusiness.Update(id, moduleDto.Name, moduleDto.Description, moduleDto.Statu);
                return Ok(new { message = "Módulo actualizado correctamente", data = updatedModule }); // 200 OK
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el módulo con ID: {ModuleId}", id);
                return BadRequest(new { message = ex.Message }); // 400 Bad Request
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Módulo no encontrado con ID: {ModuleId}", id);
                return NotFound(new { message = ex.Message }); // 404 Not Found
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el módulo con ID: {ModuleId}", id);
                return StatusCode(500, new { message = ex.Message }); // 500 Internal Server Error
            }
        }

        /// <summary>
        /// Actualiza nombre, descripción y estado de un módulo.
        /// </summary>
        [HttpPatch("{id}/Update-Name-Description-Statu")]
        [ProducesResponseType(typeof(ModuleDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateBasicInfo(int id, [FromBody] ModuleDto dto)
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
                _logger.LogWarning(ex, "Validación fallida");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Módulo no encontrado con ID: {ModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar módulo con ID: {ModuleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Activa o desactiva un módulo por ID.
        /// </summary>
        /// <param name="id">El ID del módulo.</param>
        /// <param name="isActive">Estado deseado: true para activar, false para desactivar.</param>
        /// <returns>Un código de estado indicando el resultado.</returns>
        [HttpPatch("{id}/active")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SetActiveStatus(int id, [FromBody] bool isActive)
        {
            try
            {
                var updatedModule = await _moduleBusiness.SetActiveStatusAsync(id, isActive);
                return Ok(updatedModule);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el ID del módulo: {ModuleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Módulo no encontrado con ID: {ModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al cambiar el estado del módulo con ID: {ModuleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}