using Bussines;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace WebApplication.Controllers
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
    }
}
