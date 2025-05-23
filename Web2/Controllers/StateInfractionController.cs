using Bussines;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class StateInfractionController : ControllerBase
    {
        private readonly StateInfractionBusiness _stateInfractionBusiness;
        private readonly ILogger<StateInfractionController> _logger;

        public StateInfractionController(StateInfractionBusiness stateInfractionBusiness, ILogger<StateInfractionController> logger)
        {
            _stateInfractionBusiness = stateInfractionBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las infracciones estatales.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StateInfractionDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var stateInfractions = await _stateInfractionBusiness.GetAllStateInfractionsAsync();
                return Ok(stateInfractions);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener las infracciones estatales");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una infracción estatal por ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StateInfractionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var stateInfraction = await _stateInfractionBusiness.GetStateInfractionByIdAsync(id);
                return Ok(stateInfraction);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el ID de la infracción estatal: {StateInfractionId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Infracción estatal no encontrada con ID: {StateInfractionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener la infracción estatal con ID: {StateInfractionId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva infracción estatal.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(StateInfractionDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create([FromBody] StateInfractionDto stateInfractionDto)
        {
            try
            {
                var created = await _stateInfractionBusiness.CreateStateInfractionAsync(stateInfractionDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear la infracción estatal");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear la infracción estatal");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(StateInfractionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deletedNotification = await _stateInfractionBusiness.DeleteStateInfractionAsync(id);
                return Ok(deletedNotification);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el ID de la Infraccion: {StateInfractionId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "La infraccion  no encontrada con ID: {StateInfractionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar la Infraccion  con ID: {StateInfractionId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza una infracción estatal existente.
        /// </summary>
        /// <param name="id">ID de la infracción estatal a actualizar.</param>
        /// <param name="stateInfractionDto">Datos actualizados de la infracción estatal.</param>
        /// <returns>La infracción estatal actualizada.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(StateInfractionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Update(int id, [FromBody] StateInfractionDto stateInfractionDto)
        {
            if (id != stateInfractionDto.Id)
            {
                return BadRequest(new { message = "El ID de la URL no coincide con el ID del cuerpo de la solicitud." });
            }

            try
            {
                var updated = await _stateInfractionBusiness.UpdateStateInfractionAsync(stateInfractionDto);
                return Ok(updated);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar la infracción estatal con ID: {StateInfractionId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Infracción estatal no encontrada con ID: {StateInfractionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar la infracción estatal con ID: {StateInfractionId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Activa o desactiva una infracción estatal por su ID.
        /// </summary>
        /// <param name="id">El ID de la infracción estatal.</param>
        /// <param name="isActive">Estado deseado: true para activar, false para desactivar.</param>
        /// <returns>Un código de estado indicando el resultado.</returns>
        [HttpPatch("{id}/active")]
        [ProducesResponseType(typeof(StateInfractionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SetActiveStatus(int id, [FromBody] bool isActive)
        {
            try
            {
                var updatedStateInfraction = await _stateInfractionBusiness.SetActiveStatusAsync(id, isActive);
                return Ok(updatedStateInfraction);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el ID de la infracción estatal: {StateInfractionId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Infracción estatal no encontrada con ID: {StateInfractionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al cambiar el estado de la infracción estatal con ID: {StateInfractionId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/FineValue-State")]
        [ProducesResponseType(typeof(StateInfractionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Updatee(int id, [FromBody] StateInfractionDto dto)
        {
            try
            {
                if (id != dto.Id)
                {
                    return BadRequest(new { message = "El ID en la URL no coincide con el ID en el cuerpo." });
                }

                var updated = await _stateInfractionBusiness.Update(dto.Id, dto.FineValue, dto.State);
                return Ok(updated);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Notificación no encontrada con ID: {UserNotificationId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar notificación con ID: {UserNotificationId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }


    }
}





