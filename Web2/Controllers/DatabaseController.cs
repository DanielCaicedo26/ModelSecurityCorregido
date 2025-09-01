using Microsoft.AspNetCore.Mvc;
using Web2.Services;

namespace Web2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Base de Datos")]
    public class DatabaseController : ControllerBase
    {
        private readonly IDatabaseSelectorService _databaseSelector;
        private readonly ILogger<DatabaseController> _logger;

        public DatabaseController(IDatabaseSelectorService databaseSelector, ILogger<DatabaseController> logger)
        {
            _databaseSelector = databaseSelector;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene el motor de base de datos actual
        /// </summary>
        [HttpGet("current-engine")]
        public ActionResult<object> GetCurrentEngine()
        {
            return Ok(new
            {
                engine = _databaseSelector.CurrentEngine,
                timestamp = DateTime.UtcNow,
                availableEngines = _databaseSelector.GetAvailableEngines()
            });
        }

        /// <summary>
        /// Cambia el motor de base de datos (sqlserver, postgres, mysql)
        /// </summary>
        /// <param name="engine">Motor de base de datos: sqlserver, postgres, mysql</param>
        [HttpPost("set-engine/{engine}")]
        public ActionResult<object> SetEngine(string engine)
        {
            try
            {
                var previousEngine = _databaseSelector.CurrentEngine;
                _databaseSelector.SetEngine(engine);
                
                _logger.LogInformation($"Motor de BD cambiado de {previousEngine} a {engine}");
                
                return Ok(new
                {
                    success = true,
                    message = $"Motor de base de datos cambiado exitosamente a {engine}",
                    previousEngine = previousEngine,
                    currentEngine = _databaseSelector.CurrentEngine,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Intento de cambio a motor no válido: {engine}");
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message,
                    availableEngines = _databaseSelector.GetAvailableEngines()
                });
            }
        }

        /// <summary>
        /// Obtiene todos los motores de base de datos disponibles
        /// </summary>
        [HttpGet("available-engines")]
        public ActionResult<object> GetAvailableEngines()
        {
            return Ok(new
            {
                engines = _databaseSelector.GetAvailableEngines(),
                current = _databaseSelector.CurrentEngine,
                description = new
                {
                    sqlserver = "SQL Server (Puerto 1433)",
                    postgres = "PostgreSQL (Puerto 5432)", 
                    mysql = "MySQL (Puerto 3306)"
                }
            });
        }

        /// <summary>
        /// Prueba la conexión con la base de datos actual
        /// </summary>
        [HttpGet("test-connection")]
        public async Task<ActionResult<object>> TestConnection([FromServices] IServiceProvider serviceProvider)
        {
            try
            {
                var context = _databaseSelector.GetCurrentContext(serviceProvider);
                var canConnect = await context.Database.CanConnectAsync();
                
                return Ok(new
                {
                    engine = _databaseSelector.CurrentEngine,
                    connected = canConnect,
                    status = canConnect ? "✅ Conectado" : "❌ Desconectado",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al probar conexión con {_databaseSelector.CurrentEngine}");
                return StatusCode(500, new
                {
                    engine = _databaseSelector.CurrentEngine,
                    connected = false,
                    status = "❌ Error de conexión",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
}