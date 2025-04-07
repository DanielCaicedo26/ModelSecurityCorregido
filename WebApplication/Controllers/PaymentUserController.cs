using Bussines;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PaymentUserController : ControllerBase
    {
        private readonly PaymentUserBusiness _paymentUserBusiness;
        private readonly ILogger<PaymentUserController> _logger;

        public PaymentUserController(PaymentUserBusiness paymentUserBusiness, ILogger<PaymentUserController> logger)
        {
            _paymentUserBusiness = paymentUserBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los pagos de usuarios.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PaymentUserDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var paymentUsers = await _paymentUserBusiness.GetAllPaymentUsersAsync();
                return Ok(paymentUsers);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener los pagos de usuarios");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un pago de usuario por ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PaymentUserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var paymentUser = await _paymentUserBusiness.GetPaymentUserByIdAsync(id);
                return Ok(paymentUser);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validaci�n fallida para el ID del pago de usuario: {PaymentUserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Pago de usuario no encontrado con ID: {PaymentUserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el pago de usuario con ID: {PaymentUserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo pago de usuario.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(PaymentUserDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create([FromBody] PaymentUserDto paymentUserDto)
        {
            try
            {
                var created = await _paymentUserBusiness.CreatePaymentUserAsync(paymentUserDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validaci�n fallida al crear el pago de usuario");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el pago de usuario");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}



