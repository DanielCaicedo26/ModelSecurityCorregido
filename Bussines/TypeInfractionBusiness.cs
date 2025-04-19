using Data;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Bussines
{
    /// <summary>
    /// Clase que maneja la lógica de negocio para los tipos de infracción.
    /// </summary>
    public class TypeInfractionBusiness
    {
        private readonly TypeInfractionData _typeInfractionData;
        private readonly ILogger<TypeInfractionBusiness> _logger;

        /// <summary>
        /// Constructor de la clase TypeInfractionBusiness.
        /// </summary>
        /// <param name="typeInfractionData">Instancia de TypeInfractionData para acceder a los datos de los tipos de infracción.</param>
        /// <param name="logger">Instancia de ILogger para el registro de logs.</param>
        public TypeInfractionBusiness(TypeInfractionData typeInfractionData, ILogger<TypeInfractionBusiness> logger)
        {
            _typeInfractionData = typeInfractionData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los tipos de infracción de manera asíncrona.
        /// </summary>
        /// <returns>Una lista de objetos TypeInfractionDto.</returns>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar los tipos de infracción.</exception>
        public async Task<IEnumerable<TypeInfractionDto>> GetAllTypeInfractionsAsync()
        {
            try
            {
                var typeInfractions = await _typeInfractionData.GetAllAsync();
                var visibleTypeInfractions = typeInfractions.Where(ti => ti.IsActive).ToList();
                return MapToDTOList(visibleTypeInfractions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los tipos de infracción");
                throw new ExternalServiceException("Base de datos", "Error al recuperar los tipos de infracción", ex);
            }
        }

        /// <summary>
        /// Obtiene un tipo de infracción por su ID de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID del tipo de infracción.</param>
        /// <returns>Un objeto TypeInfractionDto.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el tipo de infracción.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar el tipo de infracción.</exception>
        public async Task<TypeInfractionDto> GetTypeInfractionByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un tipo de infracción con ID inválido: {TypeInfractionId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var typeInfraction = await _typeInfractionData.GetByIdAsync(id);
                if (typeInfraction == null)
                {
                    _logger.LogInformation("No se encontró ningún tipo de infracción con ID: {TypeInfractionId}", id);
                    throw new EntityNotFoundException("TypeInfraction", id);
                }

                return MapToDTO(typeInfraction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el tipo de infracción con ID: {TypeInfractionId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el tipo de infracción con ID {id}", ex);
            }
        }

        /// <summary>
        /// Elimina un tipo de infracción por su ID de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID del tipo de infracción a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el tipo de infracción.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al eliminar el tipo de infracción.</exception>
        public async Task<bool> DeleteTypeInfractionAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un tipo de infracción con ID inválido: {TypeInfractionId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var deleted = await _typeInfractionData.DeleteAsync(id);
                if (!deleted)
                {
                    _logger.LogInformation("No se encontró ningún tipo de infracción con ID: {TypeInfractionId}", id);
                    throw new EntityNotFoundException("TypeInfraction", id);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el tipo de infracción con ID: {TypeInfractionId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el tipo de infracción con ID {id}", ex);
            }
        }
        /// <summary>
        /// Activa o desactiva un tipo de infracción por su ID.
        /// </summary>
        /// <param name="id">El ID del tipo de infracción.</param>
        /// <param name="isActive">Estado deseado: true para activar, false para desactivar.</param>
        /// <returns>El objeto TypeInfractionDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra el tipo de infracción.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar el estado.</exception>
        public async Task<TypeInfractionDto> SetActiveStatusAsync(int id, bool isActive)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó cambiar el estado de un tipo de infracción con ID inválido: {TypeInfractionId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var typeInfraction = await _typeInfractionData.GetByIdAsync(id);
                if (typeInfraction == null)
                {
                    throw new EntityNotFoundException("TypeInfraction", id);
                }

                // Actualizar el estado activo
                typeInfraction.IsActive = isActive;

                var isUpdated = await _typeInfractionData.UpdateAsync(typeInfraction);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo cambiar el estado del tipo de infracción con ID {id}");
                }

                return MapToDTO(typeInfraction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar el estado del tipo de infracción con ID: {TypeInfractionId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al cambiar el estado del tipo de infracción con ID {id}", ex);
            }
        }


        /// <summary>
        /// Actualiza los datos de un tipo de infracción.
        /// </summary>
        /// <param name="typeInfractionDto">El objeto TypeInfractionDto con los datos actualizados del tipo de infracción.</param>
        /// <returns>El objeto TypeInfractionDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si los datos son inválidos.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra el tipo de infracción.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar el tipo de infracción.</exception>
        public async Task<TypeInfractionDto> UpdateTypeInfractionAsync(TypeInfractionDto typeInfractionDto)
        {
            if (typeInfractionDto == null || typeInfractionDto.Id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar un tipo de infracción con datos inválidos o ID inválido.");
                throw new ValidationException("id", "El ID debe ser mayor que cero y los datos no pueden ser nulos.");
            }

            // Validar los datos del DTO
            ValidateTypeInfraction(typeInfractionDto);

            try
            {
                // Verificar si el tipo de infracción existe
                var existingTypeInfraction = await _typeInfractionData.GetByIdAsync(typeInfractionDto.Id);
                if (existingTypeInfraction == null)
                {
                    _logger.LogInformation("No se encontró ningún tipo de infracción con ID: {TypeInfractionId}", typeInfractionDto.Id);
                    throw new EntityNotFoundException("TypeInfraction", typeInfractionDto.Id);
                }

                // Actualizar los datos del tipo de infracción
                existingTypeInfraction.UserId = typeInfractionDto.UserId;
                existingTypeInfraction.TypeViolation = typeInfractionDto.TypeViolation;
                existingTypeInfraction.ValueInfraction = typeInfractionDto.ValueInfraction;

                var isUpdated = await _typeInfractionData.UpdateAsync(existingTypeInfraction);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar el tipo de infracción con ID {typeInfractionDto.Id}.");
                }

                return MapToDTO(existingTypeInfraction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el tipo de infracción con ID: {TypeInfractionId}", typeInfractionDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el tipo de infracción con ID {typeInfractionDto.Id}.", ex);
            }
        }





        /// <summary>
        /// Crea un nuevo tipo de infracción de manera asíncrona.
        /// </summary>
        /// <param name="typeInfractionDto">El objeto TypeInfractionDto con los datos del tipo de infracción.</param>
        /// <returns>El objeto TypeInfractionDto creado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando los datos del tipo de infracción son inválidos.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al crear el tipo de infracción.</exception>
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
                    IsActive = typeInfractionDto.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                var createdTypeInfraction = await _typeInfractionData.CreateAsync(typeInfraction);
                return MapToDTO(createdTypeInfraction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo tipo de infracción");
                throw new ExternalServiceException("Base de datos", "Error al crear el tipo de infracción", ex);
            }
        }

        /// <summary>
        /// Valida los datos del tipo de infracción.
        /// </summary>
        /// <param name="typeInfractionDto">El objeto TypeInfractionDto con los datos del tipo de infracción.</param>
        /// <exception cref="ValidationException">Lanzada cuando los datos del tipo de infracción son inválidos.</exception>
        private void ValidateTypeInfraction(TypeInfractionDto typeInfractionDto)
        {
            if (typeInfractionDto == null)
            {
                throw new ValidationException("El objeto TypeInfraction no puede ser nulo");
            }

            if (typeInfractionDto.UserId <= 0)
            {
                _logger.LogWarning("Se intentó crear un tipo de infracción con UserId inválido");
                throw new ValidationException("UserId", "El UserId debe ser mayor que cero");
            }

            if (string.IsNullOrWhiteSpace(typeInfractionDto.TypeViolation))
            {
                _logger.LogWarning("Se intentó crear un tipo de infracción con tipo de violación vacío");
                throw new ValidationException("TypeViolation", "El tipo de violación no puede estar vacío");
            }

            if (typeInfractionDto.ValueInfraction <= 0)
            {
                _logger.LogWarning("Se intentó crear un tipo de infracción con valor de infracción inválido");
                throw new ValidationException("ValueInfraction", "El valor de la infracción debe ser mayor que cero");
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
                ValueInfraction = typeInfraction.ValueInfraction,
                IsActive = typeInfraction.IsActive
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








