using Bussines;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class TypeInfractionController : ControllerBase
    {
        private readonly TypeInfractionBusiness _typeInfractionBusiness;
        private readonly ILogger<TypeInfractionController> _logger;

        public TypeInfractionController(TypeInfractionBusiness typeInfractionBusiness, ILogger<TypeInfractionController> logger)
        {
            _typeInfractionBusiness = typeInfractionBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los tipos de infracciones.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TypeInfractionDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var typeInfractions = await _typeInfractionBusiness.GetAllTypeInfractionsAsync();
                return Ok(typeInfractions);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener los tipos de infracciones");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un tipo de infracci�n por ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TypeInfractionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var typeInfraction = await _typeInfractionBusiness.GetTypeInfractionByIdAsync(id);
                return Ok(typeInfraction);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validaci�n fallida para el ID del tipo de infracci�n: {TypeInfractionId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Tipo de infracci�n no encontrado con ID: {TypeInfractionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el tipo de infracci�n con ID: {TypeInfractionId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo tipo de infracci�n.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(TypeInfractionDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create([FromBody] TypeInfractionDto typeInfractionDto)
        {
            try
            {
                var created = await _typeInfractionBusiness.CreateTypeInfractionAsync(typeInfractionDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validaci�n fallida al crear el tipo de infracci�n");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el tipo de infracci�n");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un tipo de infracci�n por su ID.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)] // No Content
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _typeInfractionBusiness.DeleteTypeInfractionAsync(id);
                if (result)
                {
                    return NoContent(); // 204 No Content
                }

                return NotFound(new { message = "Tipo de infracci�n no encontrado" }); // 404 Not Found
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validaci�n fallida al eliminar el tipo de infracci�n con ID: {TypeInfractionId}", id);
                return BadRequest(new { message = ex.Message }); // 400 Bad Request
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Tipo de infracci�n no encontrado con ID: {TypeInfractionId}", id);
                return NotFound(new { message = ex.Message }); // 404 Not Found
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el tipo de infracci�n con ID: {TypeInfractionId}", id);
                return StatusCode(500, new { message = ex.Message }); // 500 Internal Server Error
            }
        }

        /// <summary>
        /// Actualiza un tipo de infracci�n existente.
        /// </summary>
        /// <param name="id">El ID del tipo de infracci�n a actualizar.</param>
        /// <param name="typeInfractionDto">El objeto TypeInfractionDto con los datos actualizados.</param>
        /// <returns>Un resultado indicando el �xito o fallo de la operaci�n.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> Update(int id, [FromBody] TypeInfractionDto typeInfractionDto)
        {
            if (id <= 0 || typeInfractionDto == null || id != typeInfractionDto.Id)
            {
                return BadRequest(new { message = "El ID de la ruta no coincide con el ID del cuerpo de la solicitud o los datos son inv�lidos" }); // 400 Bad Request
            }

            try
            {
                var updatedTypeInfraction = await _typeInfractionBusiness.UpdateTypeInfractionAsync(typeInfractionDto);
                if (updatedTypeInfraction == null)
                {
                    return NotFound(new { message = "Tipo de infracci�n no encontrado" }); // 404 Not Found
                }

                return Ok(new { message = "Tipo de infracci�n actualizado correctamente", data = updatedTypeInfraction }); // 200 OK
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validaci�n fallida al actualizar el tipo de infracci�n con ID: {TypeInfractionId}", id);
                return BadRequest(new { message = ex.Message }); // 400 Bad Request
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Tipo de infracci�n no encontrado con ID: {TypeInfractionId}", id);
                return NotFound(new { message = ex.Message }); // 404 Not Found
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el tipo de infracci�n con ID: {TypeInfractionId}", id);
                return StatusCode(500, new { message = ex.Message }); // 500 Internal Server Error
            }
        }

        /// <summary>
        /// Activa o desactiva un tipo de infracci�n por su ID.
        /// </summary>
        /// <param name="id">El ID del tipo de infracci�n.</param>
        /// <param name="isActive">Estado deseado: true para activar, false para desactivar.</param>
        /// <returns>Un c�digo de estado indicando el resultado.</returns>
        [HttpPatch("{id}/active")]
        [ProducesResponseType(typeof(TypeInfractionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SetActiveStatus(int id, [FromBody] bool isActive)
        {
            try
            {
                var updatedTypeInfraction = await _typeInfractionBusiness.SetActiveStatusAsync(id, isActive);
                return Ok(updatedTypeInfraction);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validaci�n fallida para el ID del tipo de infracci�n: {TypeInfractionId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Tipo de infracci�n no encontrado con ID: {TypeInfractionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al cambiar el estado del tipo de infracci�n con ID: {TypeInfractionId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}







