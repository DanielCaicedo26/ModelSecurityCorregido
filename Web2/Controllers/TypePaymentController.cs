using Bussines;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class TypePaymentController : ControllerBase
    {
        private readonly TypePaymentBusiness _typePaymentBusiness;
        private readonly ILogger<TypePaymentController> _logger;

        public TypePaymentController(TypePaymentBusiness typePaymentBusiness, ILogger<TypePaymentController> logger)
        {
            _typePaymentBusiness = typePaymentBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los tipos de pago.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TypePaymentDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var typePayments = await _typePaymentBusiness.GetAllTypePaymentsAsync();
                return Ok(typePayments);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener los tipos de pago");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un tipo de pago por ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TypePaymentDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var typePayment = await _typePaymentBusiness.GetTypePaymentByIdAsync(id);
                return Ok(typePayment);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el ID del tipo de pago: {TypePaymentId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Tipo de pago no encontrado con ID: {TypePaymentId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el tipo de pago con ID: {TypePaymentId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo tipo de pago.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(TypePaymentDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create([FromBody] TypePaymentDto typePaymentDto)
        {
            try
            {
                var created = await _typePaymentBusiness.CreateTypePaymentAsync(typePaymentDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear el tipo de pago");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el tipo de pago");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un tipo de pago por ID.
        /// </summary>
        /// <param name="id">El ID del tipo de pago a eliminar.</param>
        /// <returns>Un código de estado indicando el resultado.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _typePaymentBusiness.DeleteTypePaymentAsync(id);
                return Ok(new { message = "Tipo de pago eliminado exitosamente." });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el ID del tipo de pago: {TypePaymentId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Tipo de pago no encontrado con ID: {TypePaymentId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el tipo de pago con ID: {TypePaymentId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza los datos de un tipo de pago.
        /// </summary>
        /// <param name="id">El ID del tipo de pago a actualizar.</param>
        /// <param name="typePaymentDto">El objeto con los datos actualizados del tipo de pago.</param>
        /// <returns>Un código de estado indicando el resultado.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TypePaymentDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Update(int id, [FromBody] TypePaymentDto typePaymentDto)
        {
            try
            {
                if (id != typePaymentDto.Id)
                {
                    return BadRequest(new { message = "El ID en la URL no coincide con el ID en el cuerpo de la solicitud." });
                }

                var updatedTypePayment = await _typePaymentBusiness.UpdateTypePaymentAsync(typePaymentDto);
                return Ok(updatedTypePayment);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el tipo de pago con ID: {TypePaymentId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Tipo de pago no encontrado con ID: {TypePaymentId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el tipo de pago con ID: {TypePaymentId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Activa o desactiva un tipo de pago por su ID.
        /// </summary>
        /// <param name="id">El ID del tipo de pago.</param>
        /// <param name="isActive">Estado deseado: true para activar, false para desactivar.</param>
        /// <returns>Un código de estado indicando el resultado.</returns>
        [HttpPatch("{id}/active")]
        [ProducesResponseType(typeof(TypePaymentDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SetActiveStatus(int id, [FromBody] bool isActive)
        {
            try
            {
                var updatedTypePayment = await _typePaymentBusiness.SetActiveStatusAsync(id, isActive);
                return Ok(updatedTypePayment);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el ID del tipo de pago: {TypePaymentId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Tipo de pago no encontrado con ID: {TypePaymentId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al cambiar el estado del tipo de pago con ID: {TypePaymentId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/Name-Description")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Updatee(int id, [FromBody] TypePaymentDto dto)
        {
            try
            {
                if (id != dto.Id)
                {
                    return BadRequest(new { message = "El ID en la URL no coincide con el ID en el cuerpo." });
                }

                var updated = await _typePaymentBusiness.Update(dto.Id, dto.Name, dto.Description);
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







