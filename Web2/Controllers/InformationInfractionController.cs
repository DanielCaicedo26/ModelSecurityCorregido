using Bussines;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class InformationInfractionController : ControllerBase
    {
        private readonly InformationInfractionBusiness _informationInfractionBusiness;
        private readonly ILogger<InformationInfractionController> _logger;

        public InformationInfractionController(InformationInfractionBusiness informationInfractionBusiness, ILogger<InformationInfractionController> logger)
        {
            _informationInfractionBusiness = informationInfractionBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las infracciones de informaci�n.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InformationInfractionDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var infractions = await _informationInfractionBusiness.GetAllInformationInfractionsAsync();
                return Ok(infractions);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener las infracciones de informaci�n");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una infracci�n de informaci�n por ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(InformationInfractionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var infraction = await _informationInfractionBusiness.GetInformationInfractionByIdAsync(id);
                return Ok(infraction);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validaci�n fallida para el ID de la infracci�n de informaci�n: {InfractionId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Infracci�n de informaci�n no encontrada con ID: {InfractionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener la infracci�n de informaci�n con ID: {InfractionId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva infracci�n de informaci�n.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(InformationInfractionDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create([FromBody] InformationInfractionDto informationInfractionDto)
        {
            try
            {
                var created = await _informationInfractionBusiness.CreateInformationInfractionAsync(informationInfractionDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validaci�n fallida al crear la infracci�n de informaci�n");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear la infracci�n de informaci�n");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una infracci�n de informaci�n por su ID.
        /// </summary>
        /// <param name="id">El ID de la infracci�n de informaci�n a eliminar.</param>
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
                var deletedInformationInfraction = await _informationInfractionBusiness.DeleteInformationInfractionAsync(id);
                return Ok(new { message = "Infracci�n de informaci�n eliminada correctamente", data = deletedInformationInfraction }); // 200 OK
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validaci�n fallida al eliminar la infracci�n de informaci�n con ID: {InformationInfractionId}", id);
                return BadRequest(new { message = ex.Message }); // 400 Bad Request
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Infracci�n de informaci�n no encontrada con ID: {InformationInfractionId}", id);
                return NotFound(new { message = ex.Message }); // 404 Not Found
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar la infracci�n de informaci�n con ID: {InformationInfractionId}", id);
                return StatusCode(500, new { message = ex.Message }); // 500 Internal Server Error
            }
        }

        /// <summary>
        /// Actualiza una infracci�n de informaci�n existente.
        /// </summary>
        /// <param name="id">El ID de la infracci�n de informaci�n a actualizar.</param>
        /// <param name="informationInfractionDto">El objeto InformationInfractionDto con los datos actualizados.</param>
        /// <returns>Un resultado indicando el �xito o fallo de la operaci�n.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> Update(int id, [FromBody] InformationInfractionDto informationInfractionDto)
        {
            if (id != informationInfractionDto.Id)
            {
                return BadRequest(new { message = "El ID en la URL no coincide con el ID en el cuerpo de la solicitud" });
            }

            try
            {
                var updatedInformationInfraction = await _informationInfractionBusiness.UpdateInformationInfractionAsync(informationInfractionDto);
                return Ok(new { message = "Infracci�n de informaci�n actualizada correctamente", data = updatedInformationInfraction }); // 200 OK
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validaci�n fallida al actualizar la infracci�n de informaci�n con ID: {InformationInfractionId}", id);
                return BadRequest(new { message = ex.Message }); // 400 Bad Request
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Infracci�n de informaci�n no encontrada con ID: {InformationInfractionId}", id);
                return NotFound(new { message = ex.Message }); // 404 Not Found
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar la infracci�n de informaci�n con ID: {InformationInfractionId}", id);
                return StatusCode(500, new { message = ex.Message }); // 500 Internal Server Error
            }
        }

        [HttpPatch("{id}/Update-Smldv-Values")]
        [ProducesResponseType(typeof(InformationInfractionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateSmldvValues(int id, [FromBody] InformationInfractionDto dto)
        {
            try
            {
                if (id != dto.Id)
                {
                    return BadRequest(new { message = "El ID en la URL no coincide con el ID en el cuerpo." });
                }

                var updated = await _informationInfractionBusiness.Update(
                    dto.Id,
                    dto.Numer_smldv,
                    dto.MinimumWage,
                    dto.Value_smldv,
                    dto.TotalValue
                );

                return Ok(updated);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validaci�n fallida al actualizar SMMLV");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Registro de infracci�n no encontrado con ID: {InformationInfractionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar registro de infracci�n con ID: {InformationInfractionId}", id);
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
                var updatedNotification = await _informationInfractionBusiness.SetActiveStatusAsync(id, isActive);
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
