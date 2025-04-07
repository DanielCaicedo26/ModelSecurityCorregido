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
    /// Clase que maneja la l�gica de negocio para las personas.
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
        /// Obtiene todas las personas de manera as�ncrona.
        /// </summary>
        /// <returns>Una lista de objetos PersonDto.</returns>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar las personas.</exception>
        public async Task<IEnumerable<PersonDto>> GetAllPersonsAsync()
        {
            try
            {
                var persons = await _personData.GetAllAsync();
                return MapToDTOList(persons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las personas");
                throw new ExternalServiceException("Base de datos", "Error al recuperar las personas", ex);
            }
        }

        /// <summary>
        /// Obtiene una persona por su ID de manera as�ncrona.
        /// </summary>
        /// <param name="id">El ID de la persona.</param>
        /// <returns>Un objeto PersonDto.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inv�lido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra la persona.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar la persona.</exception>
        public async Task<PersonDto> GetPersonByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� obtener una persona con ID inv�lido: {PersonId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var person = await _personData.GetByIdAsync(id);
                if (person == null)
                {
                    _logger.LogInformation("No se encontr� ninguna persona con ID: {PersonId}", id);
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
        /// Crea una nueva persona de manera as�ncrona.
        /// </summary>
        /// <param name="personDto">El objeto PersonDto con los datos de la persona.</param>
        /// <returns>El objeto PersonDto creado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando los datos de la persona son inv�lidos.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al crear la persona.</exception>
        public async Task<PersonDto> CreatePersonAsync(PersonDto personDto)
        {
            try
            {
                ValidatePerson(personDto);

                var person = new Person
                {
                    FirstName = personDto.FirstName,
                    LastName = personDto.LastName,
                    Phone = personDto.Phone,
                    CreatedAt = DateTime.UtcNow
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
        /// <exception cref="ValidationException">Lanzada cuando los datos de la persona son inv�lidos.</exception>
        private void ValidatePerson(PersonDto personDto)
        {
            if (personDto == null)
            {
                throw new ValidationException("El objeto Person no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(personDto.FirstName))
            {
                _logger.LogWarning("Se intent� crear una persona con nombre vac�o");
                throw new ValidationException("FirstName", "El nombre no puede estar vac�o");
            }

            if (string.IsNullOrWhiteSpace(personDto.LastName))
            {
                _logger.LogWarning("Se intent� crear una persona con apellido vac�o");
                throw new ValidationException("LastName", "El apellido no puede estar vac�o");
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
                Phone = person.Phone
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





