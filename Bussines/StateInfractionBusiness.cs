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
        private readonly PersonData _personData;
        private readonly ILogger<StateInfractionBusiness> _logger;

        /// <summary>
        /// Constructor de la clase StateInfractionBusiness.
        /// </summary>
        /// <param name="stateInfractionData">Instancia de StateInfractionData para acceder a los datos de las infracciones de estado.</param>
        /// <param name="personData">Instancia de PersonData para acceder a los datos de las personas.</param>
        /// <param name="logger">Instancia de ILogger para el registro de logs.</param>
        public StateInfractionBusiness(
            StateInfractionData stateInfractionData,
            PersonData personData,
            ILogger<StateInfractionBusiness> logger)
        {
            _stateInfractionData = stateInfractionData;
            _personData = personData;
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
                var visibleStateInfractions = stateInfractions.Where(si => si.IsActive);
                return MapToDTOList(visibleStateInfractions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las infracciones de estado");
                throw new ExternalServiceException("Base de datos", "Error al recuperar las infracciones de estado", ex);
            }
        }

        /// <summary>
        /// Busca infracciones por n�mero de documento de manera as�ncrona.
        /// </summary>
        public async Task<IEnumerable<StateInfractionDto>> GetInfractionsByDocumentNumberAsync(string documentNumber)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
            {
                _logger.LogWarning("Se intent� buscar infracciones con n�mero de documento vac�o");
                throw new ValidationException("documentNumber", "El n�mero de documento no puede estar vac�o");
            }

            try
            {
                // Primero intentamos buscar por el campo DocumentNumber en StateInfraction
                var infractions = await _stateInfractionData.GetByDocumentNumberAsync(documentNumber);

                // Si no hay resultados, buscamos por PersonId
                if (!infractions.Any())
                {
                    // Buscamos personas con ese n�mero de documento
                    var persons = await _personData.GetByDocumentNumberAsync(documentNumber);

                    if (persons.Any())
                    {
                        // Obtenemos las infracciones de estas personas
                        var personIds = persons.Select(p => p.Id).ToList();
                        infractions = await _stateInfractionData.GetByPersonIdsAsync(personIds);
                    }
                }

                var visibleInfractions = infractions.Where(i => i.IsActive);
                return MapToDTOList(visibleInfractions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar infracciones con n�mero de documento: {DocumentNumber}", documentNumber);
                throw new ExternalServiceException("Base de datos", $"Error al buscar infracciones con n�mero de documento {documentNumber}", ex);
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
        /// Activa o desactiva una infracci�n estatal por su ID.
        /// </summary>
        /// <param name="id">El ID de la infracci�n estatal.</param>
        /// <param name="isActive">Estado deseado: true para activar, false para desactivar.</param>
        /// <returns>El objeto StateInfractionDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si el ID es inv�lido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra la infracci�n estatal.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar el estado.</exception>
        public async Task<StateInfractionDto> SetActiveStatusAsync(int id, bool isActive)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� cambiar el estado de una infracci�n estatal con ID inv�lido: {StateInfractionId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var stateInfraction = await _stateInfractionData.GetByIdAsync(id);
                if (stateInfraction == null)
                {
                    throw new EntityNotFoundException("StateInfraction", id);
                }

                // Actualizar el estado activo
                stateInfraction.IsActive = isActive;

                var isUpdated = await _stateInfractionData.UpdateAsync(stateInfraction);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo cambiar el estado de la infracci�n estatal con ID {id}");
                }

                return MapToDTO(stateInfraction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar el estado de la infracci�n estatal con ID: {StateInfractionId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al cambiar el estado de la infracci�n estatal con ID {id}", ex);
            }
        }

        public async Task<StateInfractionDto> Update(int id, decimal FineValue, string State, string? documentNumber = null)
        {
            if (id <= 0 || string.IsNullOrWhiteSpace(State))
            {
                throw new ValidationException("Datos inv�lidos", "ID debe ser mayor que cero y el mensaje no puede estar vac�o");
            }

            try
            {
                var stateInfraction = await _stateInfractionData.GetByIdAsync(id);
                if (stateInfraction == null)
                {
                    throw new EntityNotFoundException("TypeInfraction", id);
                }

                stateInfraction.FineValue = FineValue;
                stateInfraction.State = State;

                // Si se proporciona un n�mero de documento, actualizarlo
                if (!string.IsNullOrWhiteSpace(documentNumber))
                {
                    // Verificar que exista una persona con ese documento
                    var persons = await _personData.GetByDocumentNumberAsync(documentNumber);
                    if (persons.Any())
                    {
                        var person = persons.First();
                        stateInfraction.PersonId = person.Id;
                        stateInfraction.DocumentNumber = documentNumber;
                    }
                    else
                    {
                        throw new ValidationException("DocumentNumber", $"No existe una persona con el n�mero de documento {documentNumber}");
                    }
                }

                var isUpdated = await _stateInfractionData.UpdateAsync(stateInfraction);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar la notificaci�n con ID {id}");
                }

                return MapToDTO(stateInfraction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la notificaci�n con ID: {UserNotificationId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la notificaci�n con ID {id}", ex);
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

                // Si se proporciona un n�mero de documento, comprobamos que exista una persona con ese documento
                if (!string.IsNullOrWhiteSpace(stateInfractionDto.DocumentNumber))
                {
                    var persons = await _personData.GetByDocumentNumberAsync(stateInfractionDto.DocumentNumber);
                    var person = persons.FirstOrDefault();

                    // Si se encuentra una persona con ese documento, usamos su ID
                    if (person != null)
                    {
                        stateInfractionDto.PersonId = person.Id;

                        // Verificar si la persona est� activa
                        if (!person.IsActive)
                        {
                            _logger.LogWarning("La persona con n�mero de documento {DocumentNumber} est� inactiva", stateInfractionDto.DocumentNumber);
                            throw new ValidationException("DocumentNumber", $"La persona con n�mero de documento {stateInfractionDto.DocumentNumber} est� inactiva en el sistema");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No se encontr� ninguna persona con el n�mero de documento: {DocumentNumber}", stateInfractionDto.DocumentNumber);
                        throw new ValidationException("DocumentNumber", $"No existe una persona con el n�mero de documento {stateInfractionDto.DocumentNumber}. Debe registrar primero a la persona antes de asignarle una multa.");
                    }
                }

                // Mapear los datos actualizados al modelo de entidad
                existingStateInfraction.InfractionId = stateInfractionDto.InfractionId;
                existingStateInfraction.PersonId = stateInfractionDto.PersonId;
                existingStateInfraction.DateViolation = stateInfractionDto.DateViolation;
                existingStateInfraction.FineValue = stateInfractionDto.FineValue;
                existingStateInfraction.State = stateInfractionDto.State;
                existingStateInfraction.DocumentNumber = stateInfractionDto.DocumentNumber;

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

                // Si se proporciona un n�mero de documento, comprobamos que exista una persona con ese documento
                if (!string.IsNullOrWhiteSpace(stateInfractionDto.DocumentNumber))
                {
                    var persons = await _personData.GetByDocumentNumberAsync(stateInfractionDto.DocumentNumber);
                    var person = persons.FirstOrDefault();

                    // Si se encuentra una persona con ese documento, usamos su ID
                    if (person != null)
                    {
                        stateInfractionDto.PersonId = person.Id;

                        // Verificar si la persona est� activa
                        if (!person.IsActive)
                        {
                            _logger.LogWarning("La persona con n�mero de documento {DocumentNumber} est� inactiva", stateInfractionDto.DocumentNumber);
                            throw new ValidationException("DocumentNumber", $"La persona con n�mero de documento {stateInfractionDto.DocumentNumber} est� inactiva en el sistema");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No se encontr� ninguna persona con el n�mero de documento: {DocumentNumber}", stateInfractionDto.DocumentNumber);
                        throw new ValidationException("DocumentNumber", $"No existe una persona con el n�mero de documento {stateInfractionDto.DocumentNumber}. Debe registrar primero a la persona antes de asignarle una multa.");
                    }
                }
                else if (stateInfractionDto.PersonId > 0)
                {
                    // Si no se proporciona documento pero s� un PersonId, obtenemos el documento de la persona
                    var person = await _personData.GetByIdAsync(stateInfractionDto.PersonId);
                    if (person != null)
                    {
                        stateInfractionDto.DocumentNumber = person.DocumentNumber;

                        // Verificar si la persona est� activa
                        if (!person.IsActive)
                        {
                            _logger.LogWarning("La persona con ID {PersonId} est� inactiva", stateInfractionDto.PersonId);
                            throw new ValidationException("PersonId", $"La persona con ID {stateInfractionDto.PersonId} est� inactiva en el sistema");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No se encontr� ninguna persona con el ID: {PersonId}", stateInfractionDto.PersonId);
                        throw new ValidationException("PersonId", $"No existe una persona con el ID {stateInfractionDto.PersonId}");
                    }
                }

                var stateInfraction = new StateInfraction
                {
                    InfractionId = stateInfractionDto.InfractionId,
                    PersonId = stateInfractionDto.PersonId,
                    DateViolation = stateInfractionDto.DateViolation,
                    FineValue = stateInfractionDto.FineValue,
                    State = stateInfractionDto.State,
                    DocumentNumber = stateInfractionDto.DocumentNumber,
                    IsActive = stateInfractionDto.IsActive,
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

            // Permitir la creaci�n de infracciones usando PersonId o DocumentNumber
            if (stateInfractionDto.PersonId <= 0 && string.IsNullOrWhiteSpace(stateInfractionDto.DocumentNumber))
            {
                _logger.LogWarning("Se intent� crear una infracci�n de estado sin PersonId y sin DocumentNumber");
                throw new ValidationException("PersonId/DocumentNumber", "Debe proporcionar PersonId o DocumentNumber");
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
                State = stateInfraction.State,
                DocumentNumber = stateInfraction.DocumentNumber,
                IsActive = stateInfraction.IsActive
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