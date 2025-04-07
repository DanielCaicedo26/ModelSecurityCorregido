using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Bussines
{
    /// <summary>
    /// Clase que maneja la lógica de negocio para los módulos.
    /// </summary>
    public class ModuleBusiness
    {
        private readonly ModuleData _moduleData;
        private readonly ILogger<ModuleBusiness> _logger;

        /// <summary>
        /// Constructor de la clase ModuleBusiness.
        /// </summary>
        /// <param name="moduleData">Instancia de ModuleData para acceder a los datos de los módulos.</param>
        /// <param name="logger">Instancia de ILogger para el registro de logs.</param>
        public ModuleBusiness(ModuleData moduleData, ILogger<ModuleBusiness> logger)
        {
            _moduleData = moduleData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los módulos de manera asíncrona.
        /// </summary>
        /// <returns>Una lista de objetos ModuleDto.</returns>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar los módulos.</exception>
        public async Task<IEnumerable<ModuleDto>> GetAllModulesAsync()
        {
            try
            {
                var modules = await _moduleData.GetAllAsync();
                return MapToDTOList(modules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los módulos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar los módulos", ex);
            }
        }

        /// <summary>
        /// Obtiene un módulo por su ID de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID del módulo.</param>
        /// <returns>Un objeto ModuleDto.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el módulo.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar el módulo.</exception>
        public async Task<ModuleDto> GetModuleByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un módulo con ID inválido: {ModuleId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var module = await _moduleData.GetByIdAsync(id);
                if (module == null)
                {
                    _logger.LogInformation("No se encontró ningún módulo con ID: {ModuleId}", id);
                    throw new EntityNotFoundException("Module", id);
                }

                return MapToDTO(module);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el módulo con ID: {ModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el módulo con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo módulo de manera asíncrona.
        /// </summary>
        /// <param name="moduleDto">El objeto ModuleDto con los datos del módulo.</param>
        /// <returns>El objeto ModuleDto creado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando los datos del módulo son inválidos.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al crear el módulo.</exception>
        public async Task<ModuleDto> CreateModuleAsync(ModuleDto moduleDto)
        {
            try
            {
                ValidateModule(moduleDto);

                var module = new Module
                {
                    Name = moduleDto.Name,
                    Description = moduleDto.Description,
                    Statu = moduleDto.Statu
                };

                var createdModule = await _moduleData.CreateAsync(module);
                return MapToDTO(createdModule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo módulo");
                throw new ExternalServiceException("Base de datos", "Error al crear el módulo", ex);
            }
        }

        /// <summary>
        /// Valida los datos del módulo.
        /// </summary>
        /// <param name="moduleDto">El objeto ModuleDto con los datos del módulo.</param>
        /// <exception cref="ValidationException">Lanzada cuando los datos del módulo son inválidos.</exception>
        private void ValidateModule(ModuleDto moduleDto)
        {
            if (moduleDto == null)
            {
                throw new ValidationException("El objeto Module no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(moduleDto.Name))
            {
                _logger.LogWarning("Se intentó crear un módulo con nombre vacío");
                throw new ValidationException("Name", "El nombre no puede estar vacío");
            }
        }

        /// <summary>
        /// Mapea un objeto Module a ModuleDto.
        /// </summary>
        /// <param name="module">El objeto Module a mapear.</param>
        /// <returns>El objeto ModuleDto mapeado.</returns>
        private static ModuleDto MapToDTO(Module module)
        {
            return new ModuleDto
            {
                Id = module.Id,
                Name = module.Name,
                Description = module.Description,
                Statu = module.Statu
            };
        }

        /// <summary>
        /// Mapea una lista de objetos Module a una lista de ModuleDto.
        /// </summary>
        /// <param name="modules">La lista de objetos Module a mapear.</param>
        /// <returns>La lista de objetos ModuleDto mapeados.</returns>
        private static IEnumerable<ModuleDto> MapToDTOList(IEnumerable<Module> modules)
        {
            var modulesDto = new List<ModuleDto>();
            foreach (var module in modules)
            {
                modulesDto.Add(MapToDTO(module));
            }
            return modulesDto;
        }
    }
}


