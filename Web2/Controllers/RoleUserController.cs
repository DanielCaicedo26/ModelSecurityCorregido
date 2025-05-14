using Bussines.interfaces;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RoleUserController : ControllerBase
    {
        private readonly IRoleUserBusiness _roleUserBusiness;
        private readonly ILogger<RoleUserController> _logger;

        public RoleUserController(IRoleUserBusiness roleUserBusiness, ILogger<RoleUserController> logger)
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
                var roleUsers = await _roleUserBusiness.GetAllAsync();
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
                var roleUser = await _roleUserBusiness.GetByIdAsync(id);
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
                var created = await _roleUserBusiness.CreateAsync(roleUserDto);
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

        /// <summary>
        /// Elimina un usuario de rol por su ID.
        /// </summary>
        /// <param name="id">El ID del usuario de rol a eliminar.</param>
        /// <returns>Un resultado indicando el éxito o fallo de la operación.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _roleUserBusiness.DeleteAsync(id);
                return Ok(new { message = "Usuario de rol eliminado correctamente", success = result });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar el usuario de rol con ID: {RoleUserId}", id);
                return BadRequest(new { message = ex.Message }); // 400 Bad Request
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Usuario de rol no encontrado con ID: {RoleUserId}", id);
                return NotFound(new { message = ex.Message }); // 404 Not Found
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el usuario de rol con ID: {RoleUserId}", id);
                return StatusCode(500, new { message = ex.Message }); // 500 Internal Server Error
            }
        }

        /// <summary>
        /// Actualiza un usuario de rol existente.
        /// </summary>
        /// <param name="id">El ID del usuario de rol a actualizar.</param>
        /// <param name="roleUserDto">El objeto RoleUserDto con los datos actualizados.</param>
        /// <returns>Un resultado indicando el éxito o fallo de la operación.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> Update(int id, [FromBody] RoleUserDto roleUserDto)
        {
            if (id <= 0 || roleUserDto == null || id != roleUserDto.Id)
            {
                return BadRequest(new { message = "El ID de la ruta no coincide con el ID del cuerpo de la solicitud o los datos son inválidos" });
            }

            try
            {
                var updatedRoleUser = await _roleUserBusiness.Update(id, roleUserDto);
                return Ok(new { message = "Usuario de rol actualizado correctamente", data = updatedRoleUser });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el usuario de rol con ID: {RoleUserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Usuario de rol no encontrado con ID: {RoleUserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario de rol con ID: {RoleUserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Activa o desactiva un usuario de rol por su ID.
        /// </summary>
        /// <param name="id">El ID del usuario de rol.</param>
        /// <param name="isActive">Estado deseado: true para activar, false para desactivar.</param>
        /// <returns>Un código de estado indicando el resultado.</returns>
        [HttpPatch("{id}/active")]
        [ProducesResponseType(typeof(RoleUserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SetActiveStatus(int id, [FromBody] bool isActive)
        {
            try
            {
                var updatedRoleUser = await _roleUserBusiness.SetActiveStatusAsync(id, isActive);
                return Ok(updatedRoleUser);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el ID del usuario de rol: {RoleUserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Usuario de rol no encontrado con ID: {RoleUserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al cambiar el estado del usuario de rol con ID: {RoleUserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza el RoleId y UserId de un usuario de rol por su ID.
        /// </summary>
        /// <param name="id">El ID del usuario de rol.</param>
        /// <param name="dto">El objeto RoleUserDto con los datos actualizados.</param>
        /// <returns>Un código de estado indicando el resultado.</returns>
        [HttpPatch("{id}/RoleId-UserId")]
        [ProducesResponseType(typeof(RoleUserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePartial(int id, [FromBody] RoleUserDto dto)
        {
            try
            {
                if (id != dto.Id)
                {
                    return BadRequest(new { message = "El ID en la URL no coincide con el ID en el cuerpo." });
                }

                // Primero obtenemos el objeto completo
                var currentRoleUser = await _roleUserBusiness.GetByIdAsync(id);

                // Actualizamos solo los campos específicos
                // Mantenemos todos los demás campos con sus valores actuales
                currentRoleUser.RoleId = dto.RoleId;
                currentRoleUser.UserId = dto.UserId;

                // Utilizamos el método de actualización estándar
                var updated = await _roleUserBusiness.Update(id, currentRoleUser);

                return Ok(updated);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Usuario de rol no encontrado con ID: {RoleUserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario de rol con ID: {RoleUserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}