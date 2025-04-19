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
        /// Obtiene un tipo de infracción por ID.
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
                _logger.LogWarning(ex, "Validación fallida para el ID del tipo de infracción: {TypeInfractionId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Tipo de infracción no encontrado con ID: {TypeInfractionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el tipo de infracción con ID: {TypeInfractionId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo tipo de infracción.
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
                _logger.LogWarning(ex, "Validación fallida al crear el tipo de infracción");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el tipo de infracción");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un tipo de infracción por su ID.
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

                return NotFound(new { message = "Tipo de infracción no encontrado" }); // 404 Not Found
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar el tipo de infracción con ID: {TypeInfractionId}", id);
                return BadRequest(new { message = ex.Message }); // 400 Bad Request
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Tipo de infracción no encontrado con ID: {TypeInfractionId}", id);
                return NotFound(new { message = ex.Message }); // 404 Not Found
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el tipo de infracción con ID: {TypeInfractionId}", id);
                return StatusCode(500, new { message = ex.Message }); // 500 Internal Server Error
            }
        }

        /// <summary>
        /// Actualiza un tipo de infracción existente.
        /// </summary>
        /// <param name="id">El ID del tipo de infracción a actualizar.</param>
        /// <param name="typeInfractionDto">El objeto TypeInfractionDto con los datos actualizados.</param>
        /// <returns>Un resultado indicando el éxito o fallo de la operación.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> Update(int id, [FromBody] TypeInfractionDto typeInfractionDto)
        {
            if (id <= 0 || typeInfractionDto == null || id != typeInfractionDto.Id)
            {
                return BadRequest(new { message = "El ID de la ruta no coincide con el ID del cuerpo de la solicitud o los datos son inválidos" }); // 400 Bad Request
            }

            try
            {
                var updatedTypeInfraction = await _typeInfractionBusiness.UpdateTypeInfractionAsync(typeInfractionDto);
                if (updatedTypeInfraction == null)
                {
                    return NotFound(new { message = "Tipo de infracción no encontrado" }); // 404 Not Found
                }

                return Ok(new { message = "Tipo de infracción actualizado correctamente", data = updatedTypeInfraction }); // 200 OK
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el tipo de infracción con ID: {TypeInfractionId}", id);
                return BadRequest(new { message = ex.Message }); // 400 Bad Request
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Tipo de infracción no encontrado con ID: {TypeInfractionId}", id);
                return NotFound(new { message = ex.Message }); // 404 Not Found
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el tipo de infracción con ID: {TypeInfractionId}", id);
                return StatusCode(500, new { message = ex.Message }); // 500 Internal Server Error
            }
        }

        /// <summary>
        /// Activa o desactiva un tipo de infracción por su ID.
        /// </summary>
        /// <param name="id">El ID del tipo de infracción.</param>
        /// <param name="isActive">Estado deseado: true para activar, false para desactivar.</param>
        /// <returns>Un código de estado indicando el resultado.</returns>
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
                _logger.LogWarning(ex, "Validación fallida para el ID del tipo de infracción: {TypeInfractionId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Tipo de infracción no encontrado con ID: {TypeInfractionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al cambiar el estado del tipo de infracción con ID: {TypeInfractionId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}







