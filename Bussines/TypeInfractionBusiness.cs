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
    /// Clase que maneja la l�gica de negocio para los tipos de infracci�n.
    /// </summary>
    public class TypeInfractionBusiness
    {
        private readonly TypeInfractionData _typeInfractionData;
        private readonly ILogger<TypeInfractionBusiness> _logger;

        /// <summary>
        /// Constructor de la clase TypeInfractionBusiness.
        /// </summary>
        /// <param name="typeInfractionData">Instancia de TypeInfractionData para acceder a los datos de los tipos de infracci�n.</param>
        /// <param name="logger">Instancia de ILogger para el registro de logs.</param>
        public TypeInfractionBusiness(TypeInfractionData typeInfractionData, ILogger<TypeInfractionBusiness> logger)
        {
            _typeInfractionData = typeInfractionData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los tipos de infracci�n de manera as�ncrona.
        /// </summary>
        /// <returns>Una lista de objetos TypeInfractionDto.</returns>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar los tipos de infracci�n.</exception>
        public async Task<IEnumerable<TypeInfractionDto>> GetAllTypeInfractionsAsync()
        {
            try
            {
                var typeInfractions = await _typeInfractionData.GetAllAsync();
                return MapToDTOList(typeInfractions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los tipos de infracci�n");
                throw new ExternalServiceException("Base de datos", "Error al recuperar los tipos de infracci�n", ex);
            }
        }

        /// <summary>
        /// Obtiene un tipo de infracci�n por su ID de manera as�ncrona.
        /// </summary>
        /// <param name="id">El ID del tipo de infracci�n.</param>
        /// <returns>Un objeto TypeInfractionDto.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inv�lido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el tipo de infracci�n.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar el tipo de infracci�n.</exception>
        public async Task<TypeInfractionDto> GetTypeInfractionByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� obtener un tipo de infracci�n con ID inv�lido: {TypeInfractionId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var typeInfraction = await _typeInfractionData.GetByIdAsync(id);
                if (typeInfraction == null)
                {
                    _logger.LogInformation("No se encontr� ning�n tipo de infracci�n con ID: {TypeInfractionId}", id);
                    throw new EntityNotFoundException("TypeInfraction", id);
                }

                return MapToDTO(typeInfraction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el tipo de infracci�n con ID: {TypeInfractionId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el tipo de infracci�n con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo tipo de infracci�n de manera as�ncrona.
        /// </summary>
        /// <param name="typeInfractionDto">El objeto TypeInfractionDto con los datos del tipo de infracci�n.</param>
        /// <returns>El objeto TypeInfractionDto creado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando los datos del tipo de infracci�n son inv�lidos.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al crear el tipo de infracci�n.</exception>
        public async Task<TypeInfractionDto> CreateTypeInfractionAsync(TypeInfractionDto typeInfractionDto)
        {
            try
            {
                ValidateTypeInfraction(typeInfractionDto);

                var typeInfraction = new TypeInfraction
                {
                    UserId = typeInfractionDto.UserId,
                    TypeViolation = typeInfractionDto.TypeViolation,
                    ValueInfraction = typeInfractionDto.ValueInfraction,
                    CreatedAt = DateTime.UtcNow
                };

                var createdTypeInfraction = await _typeInfractionData.CreateAsync(typeInfraction);
                return MapToDTO(createdTypeInfraction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo tipo de infracci�n");
                throw new ExternalServiceException("Base de datos", "Error al crear el tipo de infracci�n", ex);
            }
        }

        /// <summary>
        /// Valida los datos del tipo de infracci�n.
        /// </summary>
        /// <param name="typeInfractionDto">El objeto TypeInfractionDto con los datos del tipo de infracci�n.</param>
        /// <exception cref="ValidationException">Lanzada cuando los datos del tipo de infracci�n son inv�lidos.</exception>
        private void ValidateTypeInfraction(TypeInfractionDto typeInfractionDto)
        {
            if (typeInfractionDto == null)
            {
                throw new ValidationException("El objeto TypeInfraction no puede ser nulo");
            }

            if (typeInfractionDto.UserId <= 0)
            {
                _logger.LogWarning("Se intent� crear un tipo de infracci�n con UserId inv�lido");
                throw new ValidationException("UserId", "El UserId debe ser mayor que cero");
            }

            if (string.IsNullOrWhiteSpace(typeInfractionDto.TypeViolation))
            {
                _logger.LogWarning("Se intent� crear un tipo de infracci�n con tipo de violaci�n vac�o");
                throw new ValidationException("TypeViolation", "El tipo de violaci�n no puede estar vac�o");
            }

            if (typeInfractionDto.ValueInfraction <= 0)
            {
                _logger.LogWarning("Se intent� crear un tipo de infracci�n con valor de infracci�n inv�lido");
                throw new ValidationException("ValueInfraction", "El valor de la infracci�n debe ser mayor que cero");
            }
        }

        /// <summary>
        /// Mapea un objeto TypeInfraction a TypeInfractionDto.
        /// </summary>
        /// <param name="typeInfraction">El objeto TypeInfraction a mapear.</param>
        /// <returns>El objeto TypeInfractionDto mapeado.</returns>
        private static TypeInfractionDto MapToDTO(TypeInfraction typeInfraction)
        {
            return new TypeInfractionDto
            {
                Id = typeInfraction.Id,
                UserId = typeInfraction.UserId,
                TypeViolation = typeInfraction.TypeViolation,
                ValueInfraction = typeInfraction.ValueInfraction
            };
        }

        /// <summary>
        /// Mapea una lista de objetos TypeInfraction a una lista de TypeInfractionDto.
        /// </summary>
        /// <param name="typeInfractions">La lista de objetos TypeInfraction a mapear.</param>
        /// <returns>La lista de objetos TypeInfractionDto mapeados.</returns>
        private static IEnumerable<TypeInfractionDto> MapToDTOList(IEnumerable<TypeInfraction> typeInfractions)
        {
            var typeInfractionsDto = new List<TypeInfractionDto>();
            foreach (var typeInfraction in typeInfractions)
            {
                typeInfractionsDto.Add(MapToDTO(typeInfraction));
            }
            return typeInfractionsDto;
        }
    }
}








