using Bussines.interfaces;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleBusiness _roleBusiness;
        private readonly ILogger<RoleController> _logger;

        public RoleController(IRoleBusiness roleBusiness, ILogger<RoleController> logger)
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
                var roles = await _roleBusiness.GetAllAsync();
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
                var role = await _roleBusiness.GetByIdAsync(id);
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
        /// Obtiene un rol por su nombre.
        /// </summary>
        [HttpGet("by-name/{roleName}")]
        [ProducesResponseType(typeof(RoleDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetByName(string roleName)
        {
            try
            {
                var role = await _roleBusiness.GetByNameAsync(roleName);
                if (role == null)
                {
                    return NotFound(new { message = $"No se encontró ningún rol con nombre: {roleName}" });
                }
                return Ok(role);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el nombre del rol: {RoleName}", roleName);
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el rol con nombre: {RoleName}", roleName);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene todos los roles de un usuario por su ID.
        /// </summary>
        [HttpGet("by-user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<RoleDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            try
            {
                var roles = await _roleBusiness.GetByUserIdAsync(userId);
                return Ok(roles);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el ID del usuario: {UserId}", userId);
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener los roles del usuario con ID: {UserId}", userId);
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
                var created = await _roleBusiness.CreateAsync(roleDto);
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

        /// <summary>
        /// Elimina un rol por su ID.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _roleBusiness.DeleteAsync(id);
                if (!deleted)
                {
                    return NotFound(new { message = $"No se encontró ningún rol con ID: {id}" });
                }
                return Ok(new { message = "Rol eliminado correctamente" });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar el rol con ID: {RoleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Rol no encontrado con ID: {RoleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el rol con ID: {RoleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un rol existente.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Update(int id, [FromBody] RoleDto roleDto)
        {
            if (id != roleDto.Id)
            {
                return BadRequest(new { message = "El ID en la URL no coincide con el ID en el cuerpo de la solicitud" });
            }

            try
            {
                var updatedRole = await _roleBusiness.Update(id, roleDto);
                return Ok(new { message = "Rol actualizado correctamente", data = updatedRole });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el rol con ID: {RoleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Rol no encontrado con ID: {RoleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el rol con ID: {RoleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza el nombre y la descripción de un rol.
        /// </summary>
        [HttpPatch("{id}/update-name-description")]
        [ProducesResponseType(typeof(RoleDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateNameAndDescription(int id, [FromBody] RoleDto dto)
        {
            try
            {
                if (id != dto.Id)
                {
                    return BadRequest(new { message = "El ID en la URL no coincide con el ID en el cuerpo." });
                }

                var updatedRole = await _roleBusiness.Update(id, dto.RoleName, dto.Description);
                return Ok(new { message = "Rol actualizado correctamente", data = updatedRole });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el rol");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Rol no encontrado con ID: {RoleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el rol con ID: {RoleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Activa o desactiva un rol por ID.
        /// </summary>
        [HttpPatch("{id}/active")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SetActiveStatus(int id, [FromBody] bool isActive)
        {
            try
            {
                var updatedRole = await _roleBusiness.SetActiveStatusAsync(id, isActive);
                return Ok(updatedRole);
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
                _logger.LogError(ex, "Error al cambiar el estado del rol con ID: {RoleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}