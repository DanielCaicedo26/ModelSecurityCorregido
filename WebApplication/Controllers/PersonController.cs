using Bussines;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PersonController : ControllerBase
    {
        private readonly PersonBusiness _personBusiness;
        private readonly ILogger<PersonController> _logger;

        public PersonController(PersonBusiness personBusiness, ILogger<PersonController> logger)
        {
            _personBusiness = personBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las personas.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PersonDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var persons = await _personBusiness.GetAllPersonsAsync();
                return Ok(persons);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener las personas");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una persona por ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PersonDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var person = await _personBusiness.GetPersonByIdAsync(id);
                return Ok(person);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el ID de la persona: {PersonId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Persona no encontrada con ID: {PersonId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener la persona con ID: {PersonId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva persona.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(PersonDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create([FromBody] PersonDto personDto)
        {
            try
            {
                var created = await _personBusiness.CreatePersonAsync(personDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear la persona");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear la persona");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}



