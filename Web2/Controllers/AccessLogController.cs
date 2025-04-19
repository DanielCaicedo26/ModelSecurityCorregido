using Bussines;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web2.Controllers
{
    /// <summary>
    /// Controlador para la gestión de registros de acceso del sistema.
    /// </summary>
    [Route("api/[controller]")] // La ruta de la API será api/AccessLog
    [ApiController] // Marca la clase como controlador de API REST
    [Produces("application/json")] // Las respuestas serán en formato JSON
    public class AccessLogController : ControllerBase
    {
        // Dependencia a la capa de negocio
        private readonly AccessLogBusiness _accessLogBusiness;

        // Logger para registrar información, advertencias y errores
        private readonly ILogger<AccessLogController> _logger;

        /// <summary>
        /// Constructor que inyecta dependencias
        /// </summary>
        public AccessLogController(AccessLogBusiness accessLogBusiness, ILogger<AccessLogController> logger)
        {
            _accessLogBusiness = accessLogBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros de acceso
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AccessLogDto>), 200)] // Devuelve una lista de AccessLogDto en caso de éxito
        [ProducesResponseType(500)] // Devuelve error 500 si algo falla
        public async Task<IActionResult> GetAllAccessLogs()
        {
            try
            {
                // Llama a la capa de negocio para obtener todos los registros
                var logs = await _accessLogBusiness.GetAllAccessLogsAsync();

                // Devuelve los registros encontrados con código 200
                return Ok(logs);
            }
            catch (ExternalServiceException ex)
            {
                // Si ocurre un error externo, se registra y se responde con código 500
                _logger.LogError(ex, "Error al obtener los registros de acceso");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un registro de acceso por su ID
        /// </summary>
        [HttpGet("{id}")] // Ruta con parámetro (api/AccessLog/5)
        [ProducesResponseType(typeof(AccessLogDto), 200)] // Devuelve el registro encontrado
        [ProducesResponseType(400)] // Error si el ID es inválido
        [ProducesResponseType(404)] // Error si el registro no se encuentra
        [ProducesResponseType(500)] // Error interno del servidor
        public async Task<IActionResult> GetAccessLogById(int id)
        {
            try
            {
                // Llama al servicio para obtener el registro por ID
                var log = await _accessLogBusiness.GetAccessLogByIdAsync(id);

                // Si el registro se encuentra, se devuelve con código 200
                return Ok(log);
            }
            catch (ValidationException ex)
            {
                // ID inválido o fuera de rango
                _logger.LogWarning(ex, "Validación fallida para el ID: {LogId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                // No se encontró ningún registro con ese ID
                _logger.LogInformation(ex, "Registro no encontrado con ID: {LogId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                // Error inesperado
                _logger.LogError(ex, "Error al obtener el registro con ID: {LogId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo registro de acceso
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(AccessLogDto), 201)] // Devuelve el objeto creado y código 201
        [ProducesResponseType(400)] // Error si los datos no son válidos
        [ProducesResponseType(500)] // Error interno
        public async Task<IActionResult> CreateAccessLog([FromBody] AccessLogDto accessLogDto)
        {
            try
            {
                // Se asume que el userId ya está incluido en el objeto AccessLogDto
                var createdLog = await _accessLogBusiness.CreateAccessLogAsync(accessLogDto, accessLogDto.UserId);

                // Devuelve un 201 Created con la ruta del nuevo recurso
                return CreatedAtAction(nameof(GetAccessLogById), new { id = createdLog.Id }, createdLog);
            }
            catch (ValidationException ex)
            {
                // Validaciones fallidas (por ejemplo, campos requeridos vacíos)
                _logger.LogWarning(ex, "Validación fallida al crear el registro de acceso");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                // Error inesperado en la capa externa
                _logger.LogError(ex, "Error al crear el registro de acceso");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un registro de acceso existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(204)] // No Content si la actualización es exitosa
        [ProducesResponseType(400)] // Error si los datos no son válidos
        [ProducesResponseType(404)] // Error si el registro no se encuentra
        [ProducesResponseType(500)] // Error interno
        public async Task<IActionResult> UpdateAccessLog(int id, [FromBody] AccessLogDto accessLogDto)
        {
            try
            {
                if (id != accessLogDto.Id)
                {
                    return BadRequest(new { message = "El ID del cuerpo no coincide con el ID de la ruta." });
                }

                await _accessLogBusiness.UpdateAccessLogAsync(accessLogDto);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Registro no encontrado con ID: {LogId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el registro de acceso");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el registro de acceso");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un registro de acceso por su ID
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)] // No Content si la eliminación es exitosa
        [ProducesResponseType(404)] // Error si el registro no se encuentra
        [ProducesResponseType(500)] // Error interno
        public async Task<IActionResult> DeleteAccessLog(int id)
        {
            try
            {
                await _accessLogBusiness.DeleteAccessLogAsync(id);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Registro no encontrado con ID: {LogId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el registro de acceso");
                return StatusCode(500, new { message = ex.Message });
            }
        }






    }
}
