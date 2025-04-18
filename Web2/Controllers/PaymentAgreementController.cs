using Bussines;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PaymentAgreementController : ControllerBase
    {
        private readonly PaymentAgreementBusiness _paymentAgreementBusiness;
        private readonly ILogger<PaymentAgreementController> _logger;

        public PaymentAgreementController(PaymentAgreementBusiness paymentAgreementBusiness, ILogger<PaymentAgreementController> logger)
        {
            _paymentAgreementBusiness = paymentAgreementBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los acuerdos de pago.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PaymentAgreementDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var paymentAgreements = await _paymentAgreementBusiness.GetAllPaymentAgreementsAsync();
                return Ok(paymentAgreements);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener los acuerdos de pago");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un acuerdo de pago por ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PaymentAgreementDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var paymentAgreement = await _paymentAgreementBusiness.GetPaymentAgreementByIdAsync(id);
                return Ok(paymentAgreement);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el ID del acuerdo de pago: {PaymentAgreementId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Acuerdo de pago no encontrado con ID: {PaymentAgreementId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el acuerdo de pago con ID: {PaymentAgreementId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo acuerdo de pago.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(PaymentAgreementDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create([FromBody] PaymentAgreementDto paymentAgreementDto)
        {
            try
            {
                var created = await _paymentAgreementBusiness.CreatePaymentAgreementAsync(paymentAgreementDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear el acuerdo de pago");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el acuerdo de pago");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un acuerdo de pago por su ID.
        /// </summary>
        /// <param name="id">El ID del acuerdo de pago a eliminar.</param>
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
                var deletedPaymentAgreement = await _paymentAgreementBusiness.DeletePaymentAgreementAsync(id);
                return Ok(new { message = "Acuerdo de pago eliminado correctamente", data = deletedPaymentAgreement }); // 200 OK
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar el acuerdo de pago con ID: {PaymentAgreementId}", id);
                return BadRequest(new { message = ex.Message }); // 400 Bad Request
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Acuerdo de pago no encontrado con ID: {PaymentAgreementId}", id);
                return NotFound(new { message = ex.Message }); // 404 Not Found
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el acuerdo de pago con ID: {PaymentAgreementId}", id);
                return StatusCode(500, new { message = ex.Message }); // 500 Internal Server Error
            }
        }

        /// <summary>
        /// Actualiza un acuerdo de pago existente.
        /// </summary>
        /// <param name="id">El ID del acuerdo de pago a actualizar.</param>
        /// <param name="paymentAgreementDto">El objeto PaymentAgreementDto con los datos actualizados.</param>
        /// <returns>Un resultado indicando el éxito o fallo de la operación.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> Update(int id, [FromBody] PaymentAgreementDto paymentAgreementDto)
        {
            if (id != paymentAgreementDto.Id)
            {
                return BadRequest(new { message = "El ID en la URL no coincide con el ID en el cuerpo de la solicitud" });
            }

            try
            {
                var updatedPaymentAgreement = await _paymentAgreementBusiness.UpdatePaymentAgreementAsync(paymentAgreementDto);
                return Ok(new { message = "Acuerdo de pago actualizado correctamente", data = updatedPaymentAgreement }); // 200 OK
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el acuerdo de pago con ID: {PaymentAgreementId}", id);
                return BadRequest(new { message = ex.Message }); // 400 Bad Request
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Acuerdo de pago no encontrado con ID: {PaymentAgreementId}", id);
                return NotFound(new { message = ex.Message }); // 404 Not Found
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el acuerdo de pago con ID: {PaymentAgreementId}", id);
                return StatusCode(500, new { message = ex.Message }); // 500 Internal Server Error
            }
        }




    }
}


