using Bussines;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RoleFormPermissionController : ControllerBase
    {
        private readonly RoleFormPermissionBusiness _roleFormPermissionBusiness;
        private readonly ILogger<RoleFormPermissionController> _logger;

        public RoleFormPermissionController(RoleFormPermissionBusiness roleFormPermissionBusiness, ILogger<RoleFormPermissionController> logger)
        {
            _roleFormPermissionBusiness = roleFormPermissionBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los permisos de formularios de roles.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RoleFormPermissionDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var roleFormPermissions = await _roleFormPermissionBusiness.GetAllRoleFormPermissionsAsync();
                return Ok(roleFormPermissions);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener los permisos de formularios de roles");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un permiso de formulario de rol por ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RoleFormPermissionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var roleFormPermission = await _roleFormPermissionBusiness.GetRoleFormPermissionByIdAsync(id);
                return Ok(roleFormPermission);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el ID del permiso de formulario de rol: {RoleFormPermissionId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Permiso de formulario de rol no encontrado con ID: {RoleFormPermissionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el permiso de formulario de rol con ID: {RoleFormPermissionId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo permiso de formulario de rol.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(RoleFormPermissionDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create([FromBody] RoleFormPermissionDto roleFormPermissionDto)
        {
            try
            {
                var created = await _roleFormPermissionBusiness.CreateRoleFormPermissionAsync(roleFormPermissionDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear el permiso de formulario de rol");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el permiso de formulario de rol");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}




