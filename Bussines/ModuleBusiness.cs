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
    /// Clase que maneja la l�gica de negocio para los m�dulos.
    /// </summary>
    public class ModuleBusiness
    {
        private readonly ModuleData _moduleData;
        private readonly ILogger<ModuleBusiness> _logger;

        /// <summary>
        /// Constructor de la clase ModuleBusiness.
        /// </summary>
        /// <param name="moduleData">Instancia de ModuleData para acceder a los datos de los m�dulos.</param>
        /// <param name="logger">Instancia de ILogger para el registro de logs.</param>
        public ModuleBusiness(ModuleData moduleData, ILogger<ModuleBusiness> logger)
        {
            _moduleData = moduleData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los m�dulos de manera as�ncrona.
        /// </summary>
        /// <returns>Una lista de objetos ModuleDto.</returns>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar los m�dulos.</exception>
        public async Task<IEnumerable<ModuleDto>> GetAllModulesAsync()
        {
            try
            {
                var modules = await _moduleData.GetAllAsync();
                return MapToDTOList(modules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los m�dulos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar los m�dulos", ex);
            }
        }

        /// <summary>
        /// Obtiene un m�dulo por su ID de manera as�ncrona.
        /// </summary>
        /// <param name="id">El ID del m�dulo.</param>
        /// <returns>Un objeto ModuleDto.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inv�lido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el m�dulo.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar el m�dulo.</exception>
        public async Task<ModuleDto> GetModuleByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� obtener un m�dulo con ID inv�lido: {ModuleId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var module = await _moduleData.GetByIdAsync(id);
                if (module == null)
                {
                    _logger.LogInformation("No se encontr� ning�n m�dulo con ID: {ModuleId}", id);
                    throw new EntityNotFoundException("Module", id);
                }

                return MapToDTO(module);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el m�dulo con ID: {ModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el m�dulo con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo m�dulo de manera as�ncrona.
        /// </summary>
        /// <param name="moduleDto">El objeto ModuleDto con los datos del m�dulo.</param>
        /// <returns>El objeto ModuleDto creado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando los datos del m�dulo son inv�lidos.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al crear el m�dulo.</exception>
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
                _logger.LogError(ex, "Error al crear un nuevo m�dulo");
                throw new ExternalServiceException("Base de datos", "Error al crear el m�dulo", ex);
            }
        }

        /// <summary>
        /// Valida los datos del m�dulo.
        /// </summary>
        /// <param name="moduleDto">El objeto ModuleDto con los datos del m�dulo.</param>
        /// <exception cref="ValidationException">Lanzada cuando los datos del m�dulo son inv�lidos.</exception>
        private void ValidateModule(ModuleDto moduleDto)
        {
            if (moduleDto == null)
            {
                throw new ValidationException("El objeto Module no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(moduleDto.Name))
            {
                _logger.LogWarning("Se intent� crear un m�dulo con nombre vac�o");
                throw new ValidationException("Name", "El nombre no puede estar vac�o");
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


