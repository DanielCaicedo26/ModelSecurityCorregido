using Data;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Bussines
{
    /// <summary>
    /// Clase que maneja la lógica de negocio para las infracciones de información.
    /// </summary>
    public class InformationInfractionBusiness
    {
        private readonly InformationInfractionData _informationInfractionData;
        private readonly ILogger<InformationInfractionBusiness> _logger;

        /// <summary>
        /// Constructor de la clase InformationInfractionBusiness.
        /// </summary>
        /// <param name="informationInfractionData">Instancia de InformationInfractionData para acceder a los datos de las infracciones de información.</param>
        /// <param name="logger">Instancia de ILogger para el registro de logs.</param>
        public InformationInfractionBusiness(InformationInfractionData informationInfractionData, ILogger<InformationInfractionBusiness> logger)
        {
            _informationInfractionData = informationInfractionData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las infracciones de información de manera asíncrona.
        /// </summary>
        /// <returns>Una lista de objetos InformationInfractionDto.</returns>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar las infracciones de información.</exception>
        public async Task<IEnumerable<InformationInfractionDto>> GetAllInformationInfractionsAsync()
        {
            try
            {
                var informationInfractions = await _informationInfractionData.GetAllAsync();
                return MapToDTOList(informationInfractions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las infracciones de información");
                throw new ExternalServiceException("Base de datos", "Error al recuperar las infracciones de información", ex);
            }
        }

        /// <summary>
        /// Obtiene una infracción de información por su ID de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID de la infracción de información.</param>
        /// <returns>Un objeto InformationInfractionDto.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra la infracción de información.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar la infracción de información.</exception>
        public async Task<InformationInfractionDto> GetInformationInfractionByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una infracción de información con ID inválido: {InformationInfractionId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var informationInfraction = await _informationInfractionData.GetByIdAsync(id);
                if (informationInfraction == null)
                {
                    _logger.LogInformation("No se encontró ninguna infracción de información con ID: {InformationInfractionId}", id);
                    throw new EntityNotFoundException("InformationInfraction", id);
                }

                return MapToDTO(informationInfraction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la infracción de información con ID: {InformationInfractionId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la infracción de información con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea una nueva infracción de información de manera asíncrona.
        /// </summary>
        /// <param name="informationInfractionDto">El objeto InformationInfractionDto con los datos de la infracción de información.</param>
        /// <returns>El objeto InformationInfractionDto creado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando los datos de la infracción de información son inválidos.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al crear la infracción de información.</exception>
        public async Task<InformationInfractionDto> CreateInformationInfractionAsync(InformationInfractionDto informationInfractionDto)
        {
            try
            {
                ValidateInformationInfraction(informationInfractionDto);

                var informationInfraction = new InformationInfraction
                {
                    Numer_smldv = informationInfractionDto.Numer_smldv,
                    MinimumWage = informationInfractionDto.MinimumWage,
                    Value_smldv = informationInfractionDto.Value_smldv,
                    TotalValue = informationInfractionDto.TotalValue
                };

                var createdInformationInfraction = await _informationInfractionData.CreateAsync(informationInfraction);
                return MapToDTO(createdInformationInfraction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear una nueva infracción de información");
                throw new ExternalServiceException("Base de datos", "Error al crear la infracción de información", ex);
            }
        }

        /// <summary>
        /// Valida los datos de la infracción de información.
        /// </summary>
        /// <param name="informationInfractionDto">El objeto InformationInfractionDto con los datos de la infracción de información.</param>
        /// <exception cref="ValidationException">Lanzada cuando los datos de la infracción de información son inválidos.</exception>
        private void ValidateInformationInfraction(InformationInfractionDto informationInfractionDto)
        {
            if (informationInfractionDto == null)
            {
                throw new ValidationException("El objeto InformationInfraction no puede ser nulo");
            }

            if (informationInfractionDto.Numer_smldv <= 0)
            {
                _logger.LogWarning("Se intentó crear una infracción de información con número de SMLDV inválido");
                throw new ValidationException("Numer_smldv", "El número de SMLDV debe ser mayor que cero");
            }

            if (informationInfractionDto.MinimumWage <= 0)
            {
                _logger.LogWarning("Se intentó crear una infracción de información con salario mínimo inválido");
                throw new ValidationException("MinimumWage", "El salario mínimo debe ser mayor que cero");
            }

            if (informationInfractionDto.Value_smldv <= 0)
            {
                _logger.LogWarning("Se intentó crear una infracción de información con valor de SMLDV inválido");
                throw new ValidationException("Value_smldv", "El valor de SMLDV debe ser mayor que cero");
            }

            if (informationInfractionDto.TotalValue <= 0)
            {
                _logger.LogWarning("Se intentó crear una infracción de información con valor total inválido");
                throw new ValidationException("TotalValue", "El valor total debe ser mayor que cero");
            }
        }

        /// <summary>
        /// Mapea un objeto InformationInfraction a InformationInfractionDto.
        /// </summary>
        /// <param name="informationInfraction">El objeto InformationInfraction a mapear.</param>
        /// <returns>El objeto InformationInfractionDto mapeado.</returns>
        private static InformationInfractionDto MapToDTO(InformationInfraction informationInfraction)
        {
            return new InformationInfractionDto
            {
                Id = informationInfraction.Id,
                Numer_smldv = informationInfraction.Numer_smldv,
                MinimumWage = informationInfraction.MinimumWage,
                Value_smldv = informationInfraction.Value_smldv,
                TotalValue = informationInfraction.TotalValue
            };
        }

        /// <summary>
        /// Mapea una lista de objetos InformationInfraction a una lista de InformationInfractionDto.
        /// </summary>
        /// <param name="informationInfractions">La lista de objetos InformationInfraction a mapear.</param>
        /// <returns>La lista de objetos InformationInfractionDto mapeados.</returns>
        private static IEnumerable<InformationInfractionDto> MapToDTOList(IEnumerable<InformationInfraction> informationInfractions)
        {
            var informationInfractionsDto = new List<InformationInfractionDto>();
            foreach (var informationInfraction in informationInfractions)
            {
                informationInfractionsDto.Add(MapToDTO(informationInfraction));
            }
            return informationInfractionsDto;
        }
    }
}

