using Bussines.Services;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Web2.Controllers
{
    /// <summary>
    /// Controlador para la gestión de registros de acceso del sistema.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AccessLogController : ControllerBase
    {
        private readonly AccessLogBusiness _accessLogBusiness;

        public AccessLogController(AccessLogBusiness accessLogBusiness)
        {
            _accessLogBusiness = accessLogBusiness;
        }

        /// <summary>
        /// Obtiene todos los registros de acceso.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var logs = await _accessLogBusiness.GetAllAsync();
            return Ok(logs);
        }

        /// <summary>
        /// Obtiene un registro de acceso por su ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var log = await _accessLogBusiness.GetByIdAsync(id);
            return log != null ? Ok(log) : NotFound(new { message = "Registro no encontrado." });
        }

        /// <summary>
        /// Crea un nuevo registro de acceso.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AccessLogDto accessLogDto)
        {
            var createdLog = await _accessLogBusiness.CreateAsync(accessLogDto);
            return CreatedAtAction(nameof(GetById), new { id = createdLog.Id }, createdLog);
        }

        /// <summary>
        /// Actualiza un registro de acceso existente.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AccessLogDto accessLogDto)
        {
            if (id != accessLogDto.Id)
            {
                return BadRequest(new { message = "El ID del cuerpo no coincide con el ID de la ruta." });
            }

            var updatedLog = await _accessLogBusiness.Update( id,accessLogDto);
            return Ok(updatedLog);
        }

        /// <summary>
        /// Elimina un registro de acceso por su ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _accessLogBusiness.DeleteAsync(id);
            return result ? NoContent() : NotFound(new { message = "Registro no encontrado." });
        }

        /// <summary>
        /// Cambia el estado activo de un registro.
        /// </summary>
        [HttpPatch("{id}/active")]
        public async Task<IActionResult> SetActiveStatus(int id, [FromBody] bool isActive)
        {
            var updatedLog = await _accessLogBusiness.SetActiveStatusAsync(id, isActive);
            return Ok(updatedLog);
        }
    }
}
