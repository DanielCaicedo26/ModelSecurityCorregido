using Bussines;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PaymentHistoryController : ControllerBase
    {
        private readonly PaymentHistoryBusiness _paymentHistoryBusiness;
        private readonly ILogger<PaymentHistoryController> _logger;

        public PaymentHistoryController(PaymentHistoryBusiness paymentHistoryBusiness, ILogger<PaymentHistoryController> logger)
        {
            _paymentHistoryBusiness = paymentHistoryBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los historiales de pago.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PaymentHistoryDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var paymentHistories = await _paymentHistoryBusiness.GetAllPaymentHistoriesAsync();
                return Ok(paymentHistories);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener los historiales de pago");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un historial de pago por ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PaymentHistoryDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var paymentHistory = await _paymentHistoryBusiness.GetPaymentHistoryByIdAsync(id);
                return Ok(paymentHistory);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el ID del historial de pago: {PaymentHistoryId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Historial de pago no encontrado con ID: {PaymentHistoryId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el historial de pago con ID: {PaymentHistoryId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo historial de pago.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(PaymentHistoryDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create([FromBody] PaymentHistoryDto paymentHistoryDto)
        {
            try
            {
                var created = await _paymentHistoryBusiness.CreatePaymentHistoryAsync(paymentHistoryDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear el historial de pago");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el historial de pago");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un historial de pago por su ID.
        /// </summary>
        /// <param name="id">El ID del historial de pago a eliminar.</param>
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
                var deletedPaymentHistory = await _paymentHistoryBusiness.DeletePaymentHistoryAsync(id);
                return Ok(new { message = "Historial de pago eliminado correctamente", data = deletedPaymentHistory }); // 200 OK
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar el historial de pago con ID: {PaymentHistoryId}", id);
                return BadRequest(new { message = ex.Message }); // 400 Bad Request
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Historial de pago no encontrado con ID: {PaymentHistoryId}", id);
                return NotFound(new { message = ex.Message }); // 404 Not Found
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el historial de pago con ID: {PaymentHistoryId}", id);
                return StatusCode(500, new { message = ex.Message }); // 500 Internal Server Error
            }
        }

        /// <summary>
        /// Actualiza un historial de pago existente.
        /// </summary>
        /// <param name="id">El ID del historial de pago a actualizar.</param>
        /// <param name="paymentHistoryDto">El objeto PaymentHistoryDto con los datos actualizados.</param>
        /// <returns>Un resultado indicando el éxito o fallo de la operación.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> Update(int id, [FromBody] PaymentHistoryDto paymentHistoryDto)
        {
            if (id != paymentHistoryDto.Id)
            {
                return BadRequest(new { message = "El ID en la URL no coincide con el ID en el cuerpo de la solicitud" });
            }

            try
            {
                var updatedPaymentHistory = await _paymentHistoryBusiness.UpdatePaymentHistoryAsync(paymentHistoryDto);
                return Ok(new { message = "Historial de pago actualizado correctamente", data = updatedPaymentHistory }); // 200 OK
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el historial de pago con ID: {PaymentHistoryId}", id);
                return BadRequest(new { message = ex.Message }); // 400 Bad Request
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Historial de pago no encontrado con ID: {PaymentHistoryId}", id);
                return NotFound(new { message = ex.Message }); // 404 Not Found
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el historial de pago con ID: {PaymentHistoryId}", id);
                return StatusCode(500, new { message = ex.Message }); // 500 Internal Server Error
            }
        }

        /// <summary>
        /// Actualiza las propiedades Amount y PaymentDate de un historial de pago por su ID.
        /// </summary>
        /// <param name="id">El ID del historial de pago.</param>
        /// <param name="dto">El objeto PaymentHistoryDto con las propiedades a actualizar.</param>
        /// <returns>Un código de estado indicando el resultado.</returns>
        [HttpPatch("{id}/UpdateAmountAndDate")]
        [ProducesResponseType(typeof(PaymentHistoryDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateAmountAndDate(int id, [FromBody] PaymentHistoryDto dto)
        {
            try
            {
                if (id != dto.Id)
                {
                    return BadRequest(new { message = "El ID en la URL no coincide con el ID en el cuerpo." });
                }

                var updatedPaymentHistory = await _paymentHistoryBusiness.UpdateAmountAndDateAsync(dto.Id, dto.Amount, dto.PaymentDate);
                return Ok(updatedPaymentHistory);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el ID del historial de pago: {PaymentHistoryId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Historial de pago no encontrado con ID: {PaymentHistoryId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el historial de pago con ID: {PaymentHistoryId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Activa o desactiva un historial de pago por su ID.
        /// </summary>
        /// <param name="id">El ID del historial de pago.</param>
        /// <param name="isActive">Estado deseado: true para activar, false para desactivar.</param>
        /// <returns>Un código de estado indicando el resultado.</returns>
        [HttpPatch("{id}/active")]
        [ProducesResponseType(typeof(PaymentHistoryDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SetActiveStatus(int id, [FromBody] bool isActive)
        {
            try
            {
                var updatedPaymentHistory = await _paymentHistoryBusiness.SetActiveStatusAsync(id, isActive);
                return Ok(updatedPaymentHistory);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el ID del historial de pago: {PaymentHistoryId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Historial de pago no encontrado con ID: {PaymentHistoryId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al cambiar el estado del historial de pago con ID: {PaymentHistoryId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }




    }
}


