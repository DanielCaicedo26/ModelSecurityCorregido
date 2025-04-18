using Bussines;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class BillController : ControllerBase
    {
        private readonly BillBusiness _billBusiness;
        private readonly ILogger<BillController> _logger;

        public BillController(BillBusiness billBusiness, ILogger<BillController> logger)
        {
            _billBusiness = billBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las facturas.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BillDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var bills = await _billBusiness.GetAllBillsAsync();
                return Ok(bills);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener las facturas");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una factura por ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BillDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var bill = await _billBusiness.GetBillByIdAsync(id);
                return Ok(bill);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el ID de factura: {BillId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Factura no encontrada con ID: {BillId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener la factura con ID: {BillId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva factura.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(BillDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create([FromBody] BillDto billDto)
        {
            try
            {
                var created = await _billBusiness.CreateBillAsync(billDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear la factura");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear la factura");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una factura por su ID.
        /// </summary>
        /// <param name="id">El ID de la factura a eliminar.</param>
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
                var deletedBill = await _billBusiness.DeleteBillAsync(id);
                return Ok(new { message = "Factura eliminada correctamente", data = deletedBill }); // 200 OK
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar la factura con ID: {BillId}", id);
                return BadRequest(new { message = ex.Message }); // 400 Bad Request
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Factura no encontrada con ID: {BillId}", id);
                return NotFound(new { message = ex.Message }); // 404 Not Found
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar la factura con ID: {BillId}", id);
                return StatusCode(500, new { message = ex.Message }); // 500 Internal Server Error
            }
        }

        /// <summary>
        /// Actualiza una factura existente.
        /// </summary>
        /// <param name="id">El ID de la factura a actualizar.</param>
        /// <param name="billDto">El objeto BillDto con los datos actualizados.</param>
        /// <returns>Un resultado indicando el éxito o fallo de la operación.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> Update(int id, [FromBody] BillDto billDto)
        {
            if (id != billDto.Id)
            {
                return BadRequest(new { message = "El ID en la URL no coincide con el ID en el cuerpo de la solicitud" });
            }

            try
            {
                var updatedBill = await _billBusiness.UpdateBillAsync(billDto);
                return Ok(new { message = "Factura actualizada correctamente", data = updatedBill }); // 200 OK
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar la factura con ID: {BillId}", id);
                return BadRequest(new { message = ex.Message }); // 400 Bad Request
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Factura no encontrada con ID: {BillId}", id);
                return NotFound(new { message = ex.Message }); // 404 Not Found
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar la factura con ID: {BillId}", id);
                return StatusCode(500, new { message = ex.Message }); // 500 Internal Server Error
            }
        }


    }
}
