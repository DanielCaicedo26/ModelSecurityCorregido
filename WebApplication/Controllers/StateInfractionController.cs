using Bussines;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class StateInfractionController : ControllerBase
    {
        private readonly StateInfractionBusiness _stateInfractionBusiness;
        private readonly ILogger<StateInfractionController> _logger;

        public StateInfractionController(StateInfractionBusiness stateInfractionBusiness, ILogger<StateInfractionController> logger)
        {
            _stateInfractionBusiness = stateInfractionBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las infracciones estatales.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StateInfractionDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var stateInfractions = await _stateInfractionBusiness.GetAllStateInfractionsAsync();
                return Ok(stateInfractions);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener las infracciones estatales");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una infracción estatal por ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StateInfractionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var stateInfraction = await _stateInfractionBusiness.GetStateInfractionByIdAsync(id);
                return Ok(stateInfraction);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el ID de la infracción estatal: {StateInfractionId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Infracción estatal no encontrada con ID: {StateInfractionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener la infracción estatal con ID: {StateInfractionId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva infracción estatal.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(StateInfractionDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create([FromBody] StateInfractionDto stateInfractionDto)
        {
            try
            {
                var created = await _stateInfractionBusiness.CreateStateInfractionAsync(stateInfractionDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear la infracción estatal");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear la infracción estatal");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}





