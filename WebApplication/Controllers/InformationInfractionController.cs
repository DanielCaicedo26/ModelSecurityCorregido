using Bussines;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class InformationInfractionController : ControllerBase
    {
        private readonly InformationInfractionBusiness _informationInfractionBusiness;
        private readonly ILogger<InformationInfractionController> _logger;

        public InformationInfractionController(InformationInfractionBusiness informationInfractionBusiness, ILogger<InformationInfractionController> logger)
        {
            _informationInfractionBusiness = informationInfractionBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las infracciones de información.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InformationInfractionDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var infractions = await _informationInfractionBusiness.GetAllInformationInfractionsAsync();
                return Ok(infractions);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener las infracciones de información");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una infracción de información por ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(InformationInfractionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var infraction = await _informationInfractionBusiness.GetInformationInfractionByIdAsync(id);
                return Ok(infraction);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el ID de la infracción de información: {InfractionId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Infracción de información no encontrada con ID: {InfractionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener la infracción de información con ID: {InfractionId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva infracción de información.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(InformationInfractionDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create([FromBody] InformationInfractionDto informationInfractionDto)
        {
            try
            {
                var created = await _informationInfractionBusiness.CreateInformationInfractionAsync(informationInfractionDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear la infracción de información");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear la infracción de información");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
