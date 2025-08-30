using Microsoft.AspNetCore.Mvc;
using Web2.Services;

namespace Web2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Personas Dinámicas")]
    public class DynamicPersonController : ControllerBase
    {
        private readonly IDynamicPersonService _personService;
        private readonly IDatabaseSelectorService _databaseSelector;
        private readonly ILogger<DynamicPersonController> _logger;

        public DynamicPersonController(
            IDynamicPersonService personService,
            IDatabaseSelectorService databaseSelector,
            ILogger<DynamicPersonController> logger)
        {
            _personService = personService;
            _databaseSelector = databaseSelector;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las personas de la base de datos seleccionada actualmente
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                _logger.LogInformation($"Obteniendo personas desde {_databaseSelector.CurrentEngine}");
                var persons = await _personService.GetAllAsync();
                
                return Ok(new
                {
                    data = persons,
                    source = new
                    {
                        engine = _databaseSelector.CurrentEngine,
                        timestamp = DateTime.UtcNow,
                        count = persons.Count()
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener personas desde {_databaseSelector.CurrentEngine}");
                return StatusCode(500, new
                {
                    error = "Error al obtener personas",
                    source = _databaseSelector.CurrentEngine,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene una persona por ID de la base de datos seleccionada actualmente
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                _logger.LogInformation($"Obteniendo persona ID {id} desde {_databaseSelector.CurrentEngine}");
                var person = await _personService.GetByIdAsync(id);
                
                if (person == null)
                    return NotFound(new
                    {
                        message = $"Persona con ID {id} no encontrada",
                        source = _databaseSelector.CurrentEngine
                    });

                return Ok(new
                {
                    data = person,
                    source = new
                    {
                        engine = _databaseSelector.CurrentEngine,
                        timestamp = DateTime.UtcNow
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener persona ID {id} desde {_databaseSelector.CurrentEngine}");
                return StatusCode(500, new
                {
                    error = $"Error al obtener persona ID {id}",
                    source = _databaseSelector.CurrentEngine,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Compara los datos de una persona en todas las bases de datos
        /// </summary>
        [HttpGet("{id}/compare-databases")]
        public async Task<IActionResult> CompareDatabases(int id)
        {
            var results = new Dictionary<string, object>();
            var currentEngine = _databaseSelector.CurrentEngine;

            try
            {
                // Probar cada motor de base de datos
                foreach (var engine in _databaseSelector.GetAvailableEngines())
                {
                    try
                    {
                        _databaseSelector.SetEngine(engine);
                        var person = await _personService.GetByIdAsync(id);
                        
                        results[engine] = new
                        {
                            status = "success",
                            data = person,
                            found = person != null
                        };
                    }
                    catch (Exception ex)
                    {
                        results[engine] = new
                        {
                            status = "error",
                            error = ex.Message,
                            found = false
                        };
                    }
                }

                // Restaurar el motor original
                _databaseSelector.SetEngine(currentEngine);

                return Ok(new
                {
                    personId = id,
                    comparison = results,
                    currentEngine = currentEngine,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                // Asegurar que se restaure el motor original
                _databaseSelector.SetEngine(currentEngine);
                
                _logger.LogError(ex, $"Error al comparar datos de persona ID {id}");
                return StatusCode(500, new
                {
                    error = "Error al comparar datos entre bases de datos",
                    message = ex.Message
                });
            }
        }
    }
}