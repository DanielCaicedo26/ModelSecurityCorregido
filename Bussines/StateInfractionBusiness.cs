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
    /// Clase que maneja la l�gica de negocio para las infracciones de estado.
    /// </summary>
    public class StateInfractionBusiness
    {
        private readonly StateInfractionData _stateInfractionData;
        private readonly ILogger<StateInfractionBusiness> _logger;

        /// <summary>
        /// Constructor de la clase StateInfractionBusiness.
        /// </summary>
        /// <param name="stateInfractionData">Instancia de StateInfractionData para acceder a los datos de las infracciones de estado.</param>
        /// <param name="logger">Instancia de ILogger para el registro de logs.</param>
        public StateInfractionBusiness(StateInfractionData stateInfractionData, ILogger<StateInfractionBusiness> logger)
        {
            _stateInfractionData = stateInfractionData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las infracciones de estado de manera as�ncrona.
        /// </summary>
        /// <returns>Una lista de objetos StateInfractionDto.</returns>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar las infracciones de estado.</exception>
        public async Task<IEnumerable<StateInfractionDto>> GetAllStateInfractionsAsync()
        {
            try
            {
                var stateInfractions = await _stateInfractionData.GetAllAsync();
                return MapToDTOList(stateInfractions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las infracciones de estado");
                throw new ExternalServiceException("Base de datos", "Error al recuperar las infracciones de estado", ex);
            }
        }

        /// <summary>
        /// Obtiene una infracci�n de estado por su ID de manera as�ncrona.
        /// </summary>
        /// <param name="id">El ID de la infracci�n de estado.</param>
        /// <returns>Un objeto StateInfractionDto.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inv�lido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra la infracci�n de estado.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar la infracci�n de estado.</exception>
        public async Task<StateInfractionDto> GetStateInfractionByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� obtener una infracci�n de estado con ID inv�lido: {StateInfractionId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var stateInfraction = await _stateInfractionData.GetByIdAsync(id);
                if (stateInfraction == null)
                {
                    _logger.LogInformation("No se encontr� ninguna infracci�n de estado con ID: {StateInfractionId}", id);
                    throw new EntityNotFoundException("StateInfraction", id);
                }

                return MapToDTO(stateInfraction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la infracci�n de estado con ID: {StateInfractionId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la infracci�n de estado con ID {id}", ex);
            }
        }

        /// <summary>
        /// Elimina una infracci�n de estado por su ID de manera as�ncrona.
        /// </summary>
        /// <param name="id">El ID de la infracci�n de estado a eliminar.</param>
        /// <returns>Un objeto StateInfractionDto de la infracci�n eliminada.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inv�lido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra la infracci�n de estado.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al eliminar la infracci�n de estado.</exception>
        public async Task<StateInfractionDto> DeleteStateInfractionAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� eliminar una infracci�n de estado con ID inv�lido: {StateInfractionId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                // Verificar si la infracci�n de estado existe
                var stateInfraction = await _stateInfractionData.GetByIdAsync(id);
                if (stateInfraction == null)
                {
                    _logger.LogInformation("No se encontr� ninguna infracci�n de estado con ID: {StateInfractionId}", id);
                    throw new EntityNotFoundException("StateInfraction", id);
                }

                // Intentar eliminar la infracci�n de estado
                var isDeleted = await _stateInfractionData.DeleteAsync(id);
                if (!isDeleted)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo eliminar la infracci�n de estado con ID {id}");
                }

                // Devolver el objeto eliminado mapeado a DTO
                return MapToDTO(stateInfraction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la infracci�n de estado con ID: {StateInfractionId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar la infracci�n de estado con ID {id}", ex);
            }
        }

        /// <summary>
        /// Actualiza una infracci�n de estado de manera as�ncrona.
        /// </summary>
        /// <param name="stateInfractionDto">El objeto StateInfractionDto con los datos actualizados de la infracci�n de estado.</param>
        /// <returns>El objeto StateInfractionDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando los datos de la infracci�n de estado son inv�lidos.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra la infracci�n de estado.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al actualizar la infracci�n de estado.</exception>
        public async Task<StateInfractionDto> UpdateStateInfractionAsync(StateInfractionDto stateInfractionDto)
        {
            try
            {
                // Validar los datos de la infracci�n de estado
                ValidateStateInfraction(stateInfractionDto);

                // Verificar si la infracci�n de estado existe
                var existingStateInfraction = await _stateInfractionData.GetByIdAsync(stateInfractionDto.Id);
                if (existingStateInfraction == null)
                {
                    _logger.LogInformation("No se encontr� ninguna infracci�n de estado con ID: {StateInfractionId}", stateInfractionDto.Id);
                    throw new EntityNotFoundException("StateInfraction", stateInfractionDto.Id);
                }

                // Mapear los datos actualizados al modelo de entidad
                existingStateInfraction.InfractionId = stateInfractionDto.InfractionId;
                existingStateInfraction.PersonId = stateInfractionDto.PersonId;
                existingStateInfraction.DateViolation = stateInfractionDto.DateViolation;
                existingStateInfraction.FineValue = stateInfractionDto.FineValue;
                existingStateInfraction.State = stateInfractionDto.State;

                // Intentar actualizar la infracci�n de estado
                var isUpdated = await _stateInfractionData.UpdateAsync(existingStateInfraction);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar la infracci�n de estado con ID {stateInfractionDto.Id}");
                }

                // Devolver el objeto actualizado mapeado a DTO
                return MapToDTO(existingStateInfraction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la infracci�n de estado con ID: {StateInfractionId}", stateInfractionDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la infracci�n de estado con ID {stateInfractionDto.Id}", ex);
            }
        }



        /// <summary>
        /// Crea una nueva infracci�n de estado de manera as�ncrona.
        /// </summary>
        /// <param name="stateInfractionDto">El objeto StateInfractionDto con los datos de la infracci�n de estado.</param>
        /// <returns>El objeto StateInfractionDto creado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando los datos de la infracci�n de estado son inv�lidos.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al crear la infracci�n de estado.</exception>
        public async Task<StateInfractionDto> CreateStateInfractionAsync(StateInfractionDto stateInfractionDto)
        {
            try
            {
                ValidateStateInfraction(stateInfractionDto);

                var stateInfraction = new StateInfraction
                {
                    InfractionId = stateInfractionDto.InfractionId,
                    PersonId = stateInfractionDto.PersonId,
                    DateViolation = stateInfractionDto.DateViolation,
                    FineValue = stateInfractionDto.FineValue,
                    State = stateInfractionDto.State,
                    CreatedAt = DateTime.UtcNow
                };

                var createdStateInfraction = await _stateInfractionData.CreateAsync(stateInfraction);
                return MapToDTO(createdStateInfraction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear una nueva infracci�n de estado");
                throw new ExternalServiceException("Base de datos", "Error al crear la infracci�n de estado", ex);
            }
        }

        /// <summary>
        /// Valida los datos de la infracci�n de estado.
        /// </summary>
        /// <param name="stateInfractionDto">El objeto StateInfractionDto con los datos de la infracci�n de estado.</param>
        /// <exception cref="ValidationException">Lanzada cuando los datos de la infracci�n de estado son inv�lidos.</exception>
        private void ValidateStateInfraction(StateInfractionDto stateInfractionDto)
        {
            if (stateInfractionDto == null)
            {
                throw new ValidationException("El objeto StateInfraction no puede ser nulo");
            }

            if (stateInfractionDto.InfractionId <= 0)
            {
                _logger.LogWarning("Se intent� crear una infracci�n de estado con InfractionId inv�lido");
                throw new ValidationException("InfractionId", "El InfractionId debe ser mayor que cero");
            }

            if (stateInfractionDto.PersonId <= 0)
            {
                _logger.LogWarning("Se intent� crear una infracci�n de estado con PersonId inv�lido");
                throw new ValidationException("PersonId", "El PersonId debe ser mayor que cero");
            }

            if (stateInfractionDto.FineValue <= 0)
            {
                _logger.LogWarning("Se intent� crear una infracci�n de estado con valor de multa inv�lido");
                throw new ValidationException("FineValue", "El valor de la multa debe ser mayor que cero");
            }

            if (string.IsNullOrWhiteSpace(stateInfractionDto.State))
            {
                _logger.LogWarning("Se intent� crear una infracci�n de estado con estado vac�o");
                throw new ValidationException("State", "El estado no puede estar vac�o");
            }
        }

        /// <summary>
        /// Mapea un objeto StateInfraction a StateInfractionDto.
        /// </summary>
        /// <param name="stateInfraction">El objeto StateInfraction a mapear.</param>
        /// <returns>El objeto StateInfractionDto mapeado.</returns>
        private static StateInfractionDto MapToDTO(StateInfraction stateInfraction)
        {
            return new StateInfractionDto
            {
                Id = stateInfraction.Id,
                InfractionId = stateInfraction.InfractionId,
                PersonId = stateInfraction.PersonId,
                DateViolation = stateInfraction.DateViolation,
                FineValue = stateInfraction.FineValue,
                State = stateInfraction.State
            };
        }


        /// <summary>
        /// Mapea una lista de objetos StateInfraction a una lista de StateInfractionDto.
        /// </summary>
        /// <param name="stateInfractions">La lista de objetos StateInfraction a mapear.</param>
        /// <returns>La lista de objetos StateInfractionDto mapeados.</returns>
        private static IEnumerable<StateInfractionDto> MapToDTOList(IEnumerable<StateInfraction> stateInfractions)
        {
            var stateInfractionsDto = new List<StateInfractionDto>();
            foreach (var stateInfraction in stateInfractions)
            {
                stateInfractionsDto.Add(MapToDTO(stateInfraction));
            }
            return stateInfractionsDto;
        }
    }
}







