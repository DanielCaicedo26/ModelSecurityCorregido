using Bussines;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace WebApplication.Controllers
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
        /// Obtiene una notificación de usuario por ID.
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
                _logger.LogWarning(ex, "Validación fallida para el ID de la notificación de usuario: {UserNotificationId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Notificación de usuario no encontrada con ID: {UserNotificationId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener la notificación de usuario con ID: {UserNotificationId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva notificación de usuario.
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
                _logger.LogWarning(ex, "Validación fallida al crear la notificación de usuario");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear la notificación de usuario");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}









