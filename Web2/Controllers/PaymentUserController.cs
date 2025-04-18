using Bussines;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web2.Controllers
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
                _logger.LogWarning(ex, "Validación fallida para el ID del pago de usuario: {PaymentUserId}", id);
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
                _logger.LogWarning(ex, "Validación fallida al crear el pago de usuario");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el pago de usuario");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un pago de usuario por su ID.
        /// </summary>
        /// <param name="id">El ID del pago de usuario a eliminar.</param>
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
                var deletedPaymentUser = await _paymentUserBusiness.DeletePaymentUserAsync(id);
                return Ok(new { message = "Pago de usuario eliminado correctamente", data = deletedPaymentUser }); // 200 OK
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar el pago de usuario con ID: {PaymentUserId}", id);
                return BadRequest(new { message = ex.Message }); // 400 Bad Request
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Pago de usuario no encontrado con ID: {PaymentUserId}", id);
                return NotFound(new { message = ex.Message }); // 404 Not Found
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el pago de usuario con ID: {PaymentUserId}", id);
                return StatusCode(500, new { message = ex.Message }); // 500 Internal Server Error
            }
        }

        /// <summary>
        /// Actualiza un pago de usuario existente.
        /// </summary>
        /// <param name="id">El ID del pago de usuario a actualizar.</param>
        /// <param name="paymentUserDto">El objeto PaymentUserDto con los datos actualizados.</param>
        /// <returns>Un resultado indicando el éxito o fallo de la operación.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> Update(int id, [FromBody] PaymentUserDto paymentUserDto)
        {
            if (id != paymentUserDto.Id)
            {
                return BadRequest(new { message = "El ID en la URL no coincide con el ID en el cuerpo de la solicitud" });
            }

            try
            {
                var updatedPaymentUser = await _paymentUserBusiness.UpdatePaymentUserAsync(paymentUserDto);
                return Ok(new { message = "Pago de usuario actualizado correctamente", data = updatedPaymentUser }); // 200 OK
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el pago de usuario con ID: {PaymentUserId}", id);
                return BadRequest(new { message = ex.Message }); // 400 Bad Request
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Pago de usuario no encontrado con ID: {PaymentUserId}", id);
                return NotFound(new { message = ex.Message }); // 404 Not Found
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el pago de usuario con ID: {PaymentUserId}", id);
                return StatusCode(500, new { message = ex.Message }); // 500 Internal Server Error
            }
        }



    }
}



