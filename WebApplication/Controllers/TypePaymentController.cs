using Bussines;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace WebApplication.Controllers
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
    }
}







