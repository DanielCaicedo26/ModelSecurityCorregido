using Data;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Bussines
{
    /// <summary>
    /// Clase que maneja la l�gica de negocio para las infracciones de informaci�n.
    /// </summary>
    public class InformationInfractionBusiness
    {
        private readonly InformationInfractionData _informationInfractionData;
        private readonly ILogger<InformationInfractionBusiness> _logger;

        /// <summary>
        /// Constructor de la clase InformationInfractionBusiness.
        /// </summary>
        /// <param name="informationInfractionData">Instancia de InformationInfractionData para acceder a los datos de las infracciones de informaci�n.</param>
        /// <param name="logger">Instancia de ILogger para el registro de logs.</param>
        public InformationInfractionBusiness(InformationInfractionData informationInfractionData, ILogger<InformationInfractionBusiness> logger)
        {
            _informationInfractionData = informationInfractionData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las infracciones de informaci�n de manera as�ncrona.
        /// </summary>
        /// <returns>Una lista de objetos InformationInfractionDto.</returns>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar las infracciones de informaci�n.</exception>
        public async Task<IEnumerable<InformationInfractionDto>> GetAllInformationInfractionsAsync()
        {
            try
            {
                var informationInfractions = await _informationInfractionData.GetAllAsync();
                return MapToDTOList(informationInfractions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las infracciones de informaci�n");
                throw new ExternalServiceException("Base de datos", "Error al recuperar las infracciones de informaci�n", ex);
            }
        }

        /// <summary>
        /// Obtiene una infracci�n de informaci�n por su ID de manera as�ncrona.
        /// </summary>
        /// <param name="id">El ID de la infracci�n de informaci�n.</param>
        /// <returns>Un objeto InformationInfractionDto.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inv�lido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra la infracci�n de informaci�n.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar la infracci�n de informaci�n.</exception>
        public async Task<InformationInfractionDto> GetInformationInfractionByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� obtener una infracci�n de informaci�n con ID inv�lido: {InformationInfractionId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var informationInfraction = await _informationInfractionData.GetByIdAsync(id);
                if (informationInfraction == null)
                {
                    _logger.LogInformation("No se encontr� ninguna infracci�n de informaci�n con ID: {InformationInfractionId}", id);
                    throw new EntityNotFoundException("InformationInfraction", id);
                }

                return MapToDTO(informationInfraction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la infracci�n de informaci�n con ID: {InformationInfractionId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la infracci�n de informaci�n con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea una nueva infracci�n de informaci�n de manera as�ncrona.
        /// </summary>
        /// <param name="informationInfractionDto">El objeto InformationInfractionDto con los datos de la infracci�n de informaci�n.</param>
        /// <returns>El objeto InformationInfractionDto creado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando los datos de la infracci�n de informaci�n son inv�lidos.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al crear la infracci�n de informaci�n.</exception>
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
                _logger.LogError(ex, "Error al crear una nueva infracci�n de informaci�n");
                throw new ExternalServiceException("Base de datos", "Error al crear la infracci�n de informaci�n", ex);
            }
        }

        /// <summary>
        /// Valida los datos de la infracci�n de informaci�n.
        /// </summary>
        /// <param name="informationInfractionDto">El objeto InformationInfractionDto con los datos de la infracci�n de informaci�n.</param>
        /// <exception cref="ValidationException">Lanzada cuando los datos de la infracci�n de informaci�n son inv�lidos.</exception>
        private void ValidateInformationInfraction(InformationInfractionDto informationInfractionDto)
        {
            if (informationInfractionDto == null)
            {
                throw new ValidationException("El objeto InformationInfraction no puede ser nulo");
            }

            if (informationInfractionDto.Numer_smldv <= 0)
            {
                _logger.LogWarning("Se intent� crear una infracci�n de informaci�n con n�mero de SMLDV inv�lido");
                throw new ValidationException("Numer_smldv", "El n�mero de SMLDV debe ser mayor que cero");
            }

            if (informationInfractionDto.MinimumWage <= 0)
            {
                _logger.LogWarning("Se intent� crear una infracci�n de informaci�n con salario m�nimo inv�lido");
                throw new ValidationException("MinimumWage", "El salario m�nimo debe ser mayor que cero");
            }

            if (informationInfractionDto.Value_smldv <= 0)
            {
                _logger.LogWarning("Se intent� crear una infracci�n de informaci�n con valor de SMLDV inv�lido");
                throw new ValidationException("Value_smldv", "El valor de SMLDV debe ser mayor que cero");
            }

            if (informationInfractionDto.TotalValue <= 0)
            {
                _logger.LogWarning("Se intent� crear una infracci�n de informaci�n con valor total inv�lido");
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

