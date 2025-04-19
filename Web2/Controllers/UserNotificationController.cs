using Bussines;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UserNotificationController : ControllerBase
    {
        private readonly UserNotificationBusiness _userNotificationBusiness;
        private readonly ILogger<UserNotificationController> _logger;

        public UserNotificationController(UserNotificationBusiness userNotificationBusiness, ILogger<UserNotificationController> logger)
        {
            _userNotificationBusiness = userNotificationBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las notificaciones de usuarios.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserNotificationDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var userNotifications = await _userNotificationBusiness.GetAllUserNotificationsAsync();
                return Ok(userNotifications);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener las notificaciones de usuarios");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una notificaci�n de usuario por ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserNotificationDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var userNotification = await _userNotificationBusiness.GetUserNotificationByIdAsync(id);
                return Ok(userNotification);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validaci�n fallida para el ID de la notificaci�n de usuario: {UserNotificationId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Notificaci�n de usuario no encontrada con ID: {UserNotificationId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener la notificaci�n de usuario con ID: {UserNotificationId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva notificaci�n de usuario.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(UserNotificationDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create([FromBody] UserNotificationDto userNotificationDto)
        {
            try
            {
                var created = await _userNotificationBusiness.CreateUserNotificationAsync(userNotificationDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validaci�n fallida al crear la notificaci�n de usuario");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear la notificaci�n de usuario");
                return StatusCode(500, new { message = ex.Message });
            }
        }

            /// <summary>
            /// Elimina una notificaci�n de usuario por ID.
            /// </summary>
            [HttpDelete("{id}")]
            [ProducesResponseType(typeof(UserNotificationDto), 200)]
            [ProducesResponseType(400)]
            [ProducesResponseType(404)]
            [ProducesResponseType(500)]
            public async Task<IActionResult> Delete(int id)
            {
                try
                {
                    var deletedNotification = await _userNotificationBusiness.DeleteUserNotificationAsync(id);
                    return Ok(deletedNotification);
                }
                catch (ValidationException ex)
                {
                    _logger.LogWarning(ex, "Validaci�n fallida para el ID de la notificaci�n de usuario: {UserNotificationId}", id);
                    return BadRequest(new { message = ex.Message });
                }
                catch (EntityNotFoundException ex)
                {
                    _logger.LogInformation(ex, "Notificaci�n de usuario no encontrada con ID: {UserNotificationId}", id);
                    return NotFound(new { message = ex.Message });
                }
                catch (ExternalServiceException ex)
                {
                    _logger.LogError(ex, "Error al eliminar la notificaci�n de usuario con ID: {UserNotificationId}", id);
                    return StatusCode(500, new { message = ex.Message });
                }
            }


        /// <summary>
        /// Actualiza los datos de una notificaci�n de usuario.
        /// </summary>
        /// <param name="id">El ID de la notificaci�n de usuario a actualizar.</param>
        /// <param name="userNotificationDto">El objeto con los datos actualizados de la notificaci�n.</param>
        /// <returns>Un c�digo de estado indicando el resultado.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserNotificationDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Update(int id, [FromBody] UserNotificationDto userNotificationDto)
        {
            try
            {
                if (id != userNotificationDto.Id)
                {
                    return BadRequest(new { message = "El ID en la URL no coincide con el ID en el cuerpo de la solicitud." });
                }

                var updatedNotification = await _userNotificationBusiness.UpdateUserNotificationAsync(userNotificationDto);
                return Ok(updatedNotification);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validaci�n fallida al actualizar la notificaci�n de usuario con ID: {UserNotificationId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Notificaci�n de usuario no encontrada con ID: {UserNotificationId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar la notificaci�n de usuario con ID: {UserNotificationId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Activa o desactiva una notificaci�n de usuario por ID.
        /// </summary>
        /// <param name="id">El ID de la notificaci�n de usuario.</param>
        /// <param name="isActive">Estado deseado: true para activar, false para desactivar.</param>
        /// <returns>Un c�digo de estado indicando el resultado.</returns>
        [HttpPatch("{id}/active")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SetActiveStatus(int id, [FromBody] bool isActive)
        {
            try
            {
                var updatedNotification = await _userNotificationBusiness.SetActiveStatusAsync(id, isActive);
                return Ok(updatedNotification);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validaci�n fallida para el ID de la notificaci�n de usuario: {UserNotificationId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Notificaci�n de usuario no encontrada con ID: {UserNotificationId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al cambiar el estado de la notificaci�n de usuario con ID: {UserNotificationId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/message-read")]
        [ProducesResponseType(typeof(UserNotificationDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateMessageAndIsRead(int id, [FromBody] UserNotificationDto dto)
        {
            try
            {
                if (id != dto.Id)
                {
                    return BadRequest(new { message = "El ID en la URL no coincide con el ID en el cuerpo." });
                }

                var updated = await _userNotificationBusiness.UpdateMessageAndIsReadAsync(dto.Id, dto.Message, dto.IsRead);
                return Ok(updated);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validaci�n fallida");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Notificaci�n no encontrada con ID: {UserNotificationId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar notificaci�n con ID: {UserNotificationId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }












    }
}











