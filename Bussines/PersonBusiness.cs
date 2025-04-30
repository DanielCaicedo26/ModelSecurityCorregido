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
    /// Clase que maneja la lógica de negocio para las personas.
    /// </summary>
    public class PersonBusiness
    {
        private readonly PersonData _personData;
        private readonly ILogger<PersonBusiness> _logger;

        /// <summary>
        /// Constructor de la clase PersonBusiness.
        /// </summary>
        /// <param name="personData">Instancia de PersonData para acceder a los datos de las personas.</param>
        /// <param name="logger">Instancia de ILogger para el registro de logs.</param>
        public PersonBusiness(PersonData personData, ILogger<PersonBusiness> logger)
        {
            _personData = personData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las personas de manera asíncrona.
        /// </summary>
        /// <returns>Una lista de objetos PersonDto.</returns>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar las personas.</exception>
        public async Task<IEnumerable<PersonDto>> GetAllPersonsAsync()
        {
            try
            {
                var persons = await _personData.GetAllAsync();
                var visiblepersons = persons.Where(si => si.IsActive);
                return MapToDTOList(persons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las personas");
                throw new ExternalServiceException("Base de datos", "Error al recuperar las personas", ex);
            }
        }

        /// <summary>
        /// Busca personas por número de documento de manera asíncrona.
        /// </summary>
        /// <param name="documentNumber">El número de documento a buscar.</param>
        /// <returns>Una lista de objetos PersonDto que coinciden con el número de documento.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el número de documento es inválido.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar las personas.</exception>
        public async Task<IEnumerable<PersonDto>> GetPersonsByDocumentNumberAsync(string documentNumber)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
            {
                _logger.LogWarning("Se intentó buscar personas con número de documento vacío");
                throw new ValidationException("documentNumber", "El número de documento no puede estar vacío");
            }

            try
            {
                var persons = await _personData.GetByDocumentNumberAsync(documentNumber);
                var visiblePersons = persons.Where(p => p.IsActive);
                return MapToDTOList(visiblePersons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar personas con número de documento: {DocumentNumber}", documentNumber);
                throw new ExternalServiceException("Base de datos", $"Error al buscar personas con número de documento {documentNumber}", ex);
            }
        }

        /// <summary>
        /// Actualiza los datos de una persona.
        /// </summary>
        /// <param name="personDto">El objeto PersonDto con los datos actualizados de la persona.</param>
        /// <returns>El objeto PersonDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si los datos son inválidos.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra la persona.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar la persona.</exception>
        public async Task<PersonDto> UpdatePersonAsync(PersonDto personDto)
        {
            if (personDto == null || personDto.Id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar una persona con datos inválidos o ID inválido.");
                throw new ValidationException("id", "El ID debe ser mayor que cero y los datos no pueden ser nulos.");
            }

            // Validar los datos del DTO
            ValidatePerson(personDto);

            try
            {
                // Verificar si la persona existe
                var existingPerson = await _personData.GetByIdAsync(personDto.Id);
                if (existingPerson == null)
                {
                    _logger.LogInformation("No se encontró ninguna persona con ID: {PersonId}", personDto.Id);
                    throw new EntityNotFoundException("Person", personDto.Id);
                }

                // Verificar si el número de documento actualizado ya está en uso por otra persona
                if (!string.IsNullOrWhiteSpace(personDto.DocumentNumber) &&
                    existingPerson.DocumentNumber != personDto.DocumentNumber)
                {
                    var personsWithSameDocument = await _personData.GetByDocumentNumberAsync(personDto.DocumentNumber);
                    var otherPersonWithSameDocument = personsWithSameDocument.FirstOrDefault(p => p.Id != personDto.Id);

                    if (otherPersonWithSameDocument != null)
                    {
                        _logger.LogWarning("Ya existe otra persona con el número de documento: {DocumentNumber}", personDto.DocumentNumber);
                        throw new ValidationException("DocumentNumber", $"Ya existe otra persona con el número de documento {personDto.DocumentNumber}. Este campo debe ser único para cada persona en el sistema.");
                    }
                }

                // Validar que no se esté intentando eliminar el número de documento
                if (string.IsNullOrWhiteSpace(personDto.DocumentNumber))
                {
                    _logger.LogWarning("Se intentó actualizar una persona eliminando su número de documento");
                    throw new ValidationException("DocumentNumber", "El número de documento es obligatorio y no puede estar vacío");
                }

                // Actualizar los datos de la persona
                existingPerson.FirstName = personDto.FirstName;
                existingPerson.LastName = personDto.LastName;
                existingPerson.Phone = personDto.Phone;
                existingPerson.DocumentNumber = personDto.DocumentNumber;
                existingPerson.DocumentType = personDto.DocumentType;
                existingPerson.IsActive = personDto.IsActive;

                var isUpdated = await _personData.UpdateAsync(existingPerson);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar la persona con ID {personDto.Id}.");
                }

                return MapToDTO(existingPerson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la persona con ID: {PersonId}", personDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la persona con ID {personDto.Id}.", ex);
            }
        }

        /// <summary>
        /// Elimina una persona por su ID de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID de la persona a eliminar.</param>
        /// <returns>Un objeto PersonDto de la persona eliminada.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra la persona.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al eliminar la persona.</exception>
        public async Task<PersonDto> DeletePersonAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar una persona con ID inválido: {PersonId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                // Verificar si la persona existe
                var person = await _personData.GetByIdAsync(id);
                if (person == null)
                {
                    _logger.LogInformation("No se encontró ninguna persona con ID: {PersonId}", id);
                    throw new EntityNotFoundException("Person", id);
                }

                // Intentar eliminar la persona
                var isDeleted = await _personData.DeleteAsync(id);
                if (!isDeleted)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo eliminar la persona con ID {id}");
                }

                // Devolver el objeto eliminado mapeado a DTO
                return MapToDTO(person);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la persona con ID: {PersonId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar la persona con ID {id}", ex);
            }
        }

        /// <summary>
        /// Actualiza propiedades específicas de una persona.
        /// </summary>
        /// <param name="id">El ID de la persona.</param>
        /// <param name="firstName">El nuevo nombre de la persona.</param>
        /// <param name="lastName">El nuevo apellido de la persona.</param>
        /// <returns>El objeto PersonDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si los datos son inválidos.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra la persona.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar la persona.</exception>
        public async Task<PersonDto> UpdatePartialAsync(int id, string? firstName, string? lastName)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar una persona con ID inválido.");
                throw new ValidationException("id", "El ID debe ser mayor que cero.");
            }

            try
            {
                var existingPerson = await _personData.GetByIdAsync(id);
                if (existingPerson == null)
                {
                    throw new EntityNotFoundException("Person", id);
                }

                // Actualizar solo las propiedades proporcionadas
                if (!string.IsNullOrWhiteSpace(firstName))
                {
                    existingPerson.FirstName = firstName;
                }

                if (!string.IsNullOrWhiteSpace(lastName))
                {
                    existingPerson.LastName = lastName;
                }

                var isUpdated = await _personData.UpdateAsync(existingPerson);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar la persona con ID {id}.");
                }

                return MapToDTO(existingPerson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la persona con ID: {PersonId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la persona con ID {id}.", ex);
            }
        }

        /// <summary>
        /// Actualiza propiedades específicas de una persona, incluyendo el documento.
        /// </summary>
        /// <param name="id">El ID de la persona.</param>
        /// <param name="documentNumber">El nuevo número de documento.</param>
        /// <param name="documentType">El nuevo tipo de documento.</param>
        /// <returns>El objeto PersonDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si los datos son inválidos.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra la persona.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar la persona.</exception>
        public async Task<PersonDto> UpdateDocumentAsync(int id, string documentNumber, string? documentType)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar una persona con ID inválido.");
                throw new ValidationException("id", "El ID debe ser mayor que cero.");
            }

            if (string.IsNullOrWhiteSpace(documentNumber))
            {
                _logger.LogWarning("Se intentó actualizar una persona con número de documento vacío.");
                throw new ValidationException("documentNumber", "El número de documento no puede estar vacío.");
            }

            try
            {
                var existingPerson = await _personData.GetByIdAsync(id);
                if (existingPerson == null)
                {
                    throw new EntityNotFoundException("Person", id);
                }

                // Verificar si el número de documento ya existe
                if (existingPerson.DocumentNumber != documentNumber)
                {
                    var personsWithSameDocument = await _personData.GetByDocumentNumberAsync(documentNumber);
                    if (personsWithSameDocument.Any(p => p.Id != id))
                    {
                        throw new ValidationException("documentNumber", $"Ya existe otra persona con el número de documento {documentNumber}");
                    }
                }

                // Actualizar el documento
                existingPerson.DocumentNumber = documentNumber;
                if (!string.IsNullOrWhiteSpace(documentType))
                {
                    existingPerson.DocumentType = documentType;
                }

                var isUpdated = await _personData.UpdateAsync(existingPerson);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar la persona con ID {id}.");
                }

                return MapToDTO(existingPerson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el documento de la persona con ID: {PersonId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el documento de la persona con ID {id}.", ex);
            }
        }

        /// <summary>
        /// Activa o desactiva una persona por su ID.
        /// </summary>
        /// <param name="id">El ID de la persona.</param>
        /// <param name="isActive">Estado deseado: true para activar, false para desactivar.</param>
        /// <returns>El objeto PersonDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra la persona.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar el estado.</exception>
        public async Task<PersonDto> SetActiveStatusAsync(int id, bool isActive)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó cambiar el estado de una persona con ID inválido.");
                throw new ValidationException("id", "El ID debe ser mayor que cero.");
            }

            try
            {
                var person = await _personData.GetByIdAsync(id);
                if (person == null)
                {
                    throw new EntityNotFoundException("Person", id);
                }

                // Actualizar el estado activo
                person.IsActive = isActive;

                var isUpdated = await _personData.UpdateAsync(person);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo cambiar el estado de la persona con ID {id}.");
                }

                return MapToDTO(person);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar el estado de la persona con ID: {PersonId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al cambiar el estado de la persona con ID {id}.", ex);
            }
        }

        /// <summary>
        /// Obtiene una persona por su ID de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID de la persona.</param>
        /// <returns>Un objeto PersonDto.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra la persona.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar la persona.</exception>
        public async Task<PersonDto> GetPersonByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una persona con ID inválido: {PersonId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var person = await _personData.GetByIdAsync(id);
                if (person == null)
                {
                    _logger.LogInformation("No se encontró ninguna persona con ID: {PersonId}", id);
                    throw new EntityNotFoundException("Person", id);
                }

                return MapToDTO(person);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la persona con ID: {PersonId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la persona con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea una nueva persona de manera asíncrona.
        /// </summary>
        /// <param name="personDto">El objeto PersonDto con los datos de la persona.</param>
        /// <returns>El objeto PersonDto creado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando los datos de la persona son inválidos.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al crear la persona.</exception>
        public async Task<PersonDto> CreatePersonAsync(PersonDto personDto)
        {
            try
            {
                ValidatePerson(personDto);

                // Verificar si ya existe una persona con el mismo número de documento
                if (!string.IsNullOrWhiteSpace(personDto.DocumentNumber))
                {
                    var existingPersons = await _personData.GetByDocumentNumberAsync(personDto.DocumentNumber);
                    if (existingPersons.Any())
                    {
                        _logger.LogWarning("Ya existe una persona con el número de documento: {DocumentNumber}", personDto.DocumentNumber);
                        throw new ValidationException("DocumentNumber", $"Ya existe una persona con el número de documento {personDto.DocumentNumber}. El número de documento debe ser único en el sistema.");
                    }
                }

                var person = new Person
                {
                    FirstName = personDto.FirstName,
                    LastName = personDto.LastName,
                    DocumentNumber = personDto.DocumentNumber,
                    DocumentType = personDto.DocumentType,
                    Phone = personDto.Phone,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = personDto.IsActive
                };

                var createdPerson = await _personData.CreateAsync(person);
                return MapToDTO(createdPerson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear una nueva persona");
                throw new ExternalServiceException("Base de datos", "Error al crear la persona", ex);
            }
        }

        /// <summary>
        /// Valida los datos de la persona.
        /// </summary>
        /// <param name="personDto">El objeto PersonDto con los datos de la persona.</param>
        /// <exception cref="ValidationException">Lanzada cuando los datos de la persona son inválidos.</exception>
        private void ValidatePerson(PersonDto personDto)
        {
            if (personDto == null)
            {
                throw new ValidationException("El objeto Person no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(personDto.FirstName))
            {
                _logger.LogWarning("Se intentó crear una persona con nombre vacío");
                throw new ValidationException("FirstName", "El nombre no puede estar vacío");
            }

            if (string.IsNullOrWhiteSpace(personDto.LastName))
            {
                _logger.LogWarning("Se intentó crear una persona con apellido vacío");
                throw new ValidationException("LastName", "El apellido no puede estar vacío");
            }

            // Validar el número de documento
            if (string.IsNullOrWhiteSpace(personDto.DocumentNumber))
            {
                _logger.LogWarning("Se intentó crear/actualizar una persona sin número de documento");
                throw new ValidationException("DocumentNumber", "El número de documento es obligatorio y no puede estar vacío");
            }

            if (personDto.DocumentNumber.Length < 5 || personDto.DocumentNumber.Length > 20)
            {
                _logger.LogWarning("Se intentó crear/actualizar una persona con número de documento de longitud inválida");
                throw new ValidationException("DocumentNumber", "El número de documento debe tener entre 5 y 20 caracteres");
            }

            // Validar que el número de documento solo contenga caracteres alfanuméricos
            if (!personDto.DocumentNumber.All(c => char.IsLetterOrDigit(c) || c == '-'))
            {
                _logger.LogWarning("Se intentó crear/actualizar una persona con caracteres inválidos en el número de documento");
                throw new ValidationException("DocumentNumber", "El número de documento solo puede contener letras, números y guiones");
            }
        }

        /// <summary>
        /// Mapea un objeto Person a PersonDto.
        /// </summary>
        /// <param name="person">El objeto Person a mapear.</param>
        /// <returns>El objeto PersonDto mapeado.</returns>
        private static PersonDto MapToDTO(Person person)
        {
            return new PersonDto
            {
                Id = person.Id,
                FirstName = person.FirstName,
                LastName = person.LastName,
                DocumentNumber = person.DocumentNumber,
                DocumentType = person.DocumentType,
                Phone = person.Phone,
                IsActive = person.IsActive
            };
        }

        /// <summary>
        /// Mapea una lista de objetos Person a una lista de PersonDto.
        /// </summary>
        /// <param name="persons">La lista de objetos Person a mapear.</param>
        /// <returns>La lista de objetos PersonDto mapeados.</returns>
        private static IEnumerable<PersonDto> MapToDTOList(IEnumerable<Person> persons)
        {
            var personsDto = new List<PersonDto>();
            foreach (var person in persons)
            {
                personsDto.Add(MapToDTO(person));
            }
            return personsDto;
        }
    }
}