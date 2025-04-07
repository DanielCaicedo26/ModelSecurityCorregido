using Bussines;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RoleController : ControllerBase
    {
        private readonly RoleBusiness _roleBusiness;
        private readonly ILogger<RoleController> _logger;

        public RoleController(RoleBusiness roleBusiness, ILogger<RoleController> logger)
        {
            _roleBusiness = roleBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los roles.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RoleDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var roles = await _roleBusiness.GetAllRolesAsync();
                return Ok(roles);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener los roles");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un rol por ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RoleDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var role = await _roleBusiness.GetRoleByIdAsync(id);
                return Ok(role);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el ID del rol: {RoleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Rol no encontrado con ID: {RoleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el rol con ID: {RoleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo rol.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(RoleDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create([FromBody] RoleDto roleDto)
        {
            try
            {
                var created = await _roleBusiness.CreateRoleAsync(roleDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear el rol");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el rol");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}




