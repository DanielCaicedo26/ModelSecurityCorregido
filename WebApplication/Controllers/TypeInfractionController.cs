using Bussines;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class TypeInfractionController : ControllerBase
    {
        private readonly TypeInfractionBusiness _typeInfractionBusiness;
        private readonly ILogger<TypeInfractionController> _logger;

        public TypeInfractionController(TypeInfractionBusiness typeInfractionBusiness, ILogger<TypeInfractionController> logger)
        {
            _typeInfractionBusiness = typeInfractionBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los tipos de infracciones.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TypeInfractionDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var typeInfractions = await _typeInfractionBusiness.GetAllTypeInfractionsAsync();
                return Ok(typeInfractions);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener los tipos de infracciones");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un tipo de infracción por ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TypeInfractionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var typeInfraction = await _typeInfractionBusiness.GetTypeInfractionByIdAsync(id);
                return Ok(typeInfraction);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el ID del tipo de infracción: {TypeInfractionId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Tipo de infracción no encontrado con ID: {TypeInfractionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el tipo de infracción con ID: {TypeInfractionId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo tipo de infracción.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(TypeInfractionDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create([FromBody] TypeInfractionDto typeInfractionDto)
        {
            try
            {
                var created = await _typeInfractionBusiness.CreateTypeInfractionAsync(typeInfractionDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear el tipo de infracción");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el tipo de infracción");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}






