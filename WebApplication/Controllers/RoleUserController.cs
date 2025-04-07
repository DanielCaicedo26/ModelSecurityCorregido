using Bussines;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RoleUserController : ControllerBase
    {
        private readonly RoleUserBusiness _roleUserBusiness;
        private readonly ILogger<RoleUserController> _logger;

        public RoleUserController(RoleUserBusiness roleUserBusiness, ILogger<RoleUserController> logger)
        {
            _roleUserBusiness = roleUserBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los roles de usuarios.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RoleUserDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var roleUsers = await _roleUserBusiness.GetAllRoleUsersAsync();
                return Ok(roleUsers);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener los roles de usuarios");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un rol de usuario por ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RoleUserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var roleUser = await _roleUserBusiness.GetRoleUserByIdAsync(id);
                return Ok(roleUser);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el ID del rol de usuario: {RoleUserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Rol de usuario no encontrado con ID: {RoleUserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el rol de usuario con ID: {RoleUserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo rol de usuario.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(RoleUserDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create([FromBody] RoleUserDto roleUserDto)
        {
            try
            {
                var created = await _roleUserBusiness.CreateRoleUserAsync(roleUserDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear el rol de usuario");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el rol de usuario");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}





