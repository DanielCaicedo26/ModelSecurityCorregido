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
    }
}


