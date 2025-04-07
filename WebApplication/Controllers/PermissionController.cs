using Bussines;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PermissionController : ControllerBase
    {
        private readonly PermissionBusiness _permissionBusiness;
        private readonly ILogger<PermissionController> _logger;

        public PermissionController(PermissionBusiness permissionBusiness, ILogger<PermissionController> logger)
        {
            _permissionBusiness = permissionBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los permisos.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PermissionDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var permissions = await _permissionBusiness.GetAllPermissionsAsync();
                return Ok(permissions);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener los permisos");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un permiso por ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PermissionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var permission = await _permissionBusiness.GetPermissionByIdAsync(id);
                return Ok(permission);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el ID del permiso: {PermissionId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Permiso no encontrado con ID: {PermissionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el permiso con ID: {PermissionId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo permiso.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(PermissionDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create([FromBody] PermissionDto permissionDto)
        {
            try
            {
                var created = await _permissionBusiness.CreatePermissionAsync(permissionDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear el permiso");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el permiso");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}



