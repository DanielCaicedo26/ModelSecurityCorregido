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
                var visbibleinformationInfractions = informationInfractions.Where(n => n.IsActive);
                return MapToDTOList(visbibleinformationInfractions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las infracciones de información");
                throw new ExternalServiceException("Base de datos", "Error al recuperar las infracciones de información", ex);
            }
        }

        /// <summary>
        /// Elimina una infracción de información por su ID de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID de la infracción de información a eliminar.</param>
        /// <returns>Un objeto InformationInfractionDto de la infracción de información eliminada.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra la infracción de información.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al eliminar la infracción de información.</exception>
        public async Task<InformationInfractionDto> DeleteInformationInfractionAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar una infracción de información con ID inválido: {InformationInfractionId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                // Verificar si la infracción de información existe
                var informationInfraction = await _informationInfractionData.GetByIdAsync(id);
                if (informationInfraction == null)
                {
                    _logger.LogInformation("No se encontró ninguna infracción de información con ID: {InformationInfractionId}", id);
                    throw new EntityNotFoundException("InformationInfraction", id);
                }

                // Intentar eliminar la infracción de información
                var isDeleted = await _informationInfractionData.DeleteAsync(id);
                if (!isDeleted)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo eliminar la infracción de información con ID {id}");
                }

                // Devolver el objeto eliminado mapeado a DTO
                return MapToDTO(informationInfraction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la infracción de información con ID: {InformationInfractionId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar la infracción de información con ID {id}", ex);
            }
        }

        public async Task<InformationInfractionDto> Update(int id, int numer_smldv, decimal minimumWage, decimal value_smldv, decimal totalValue)
        {
            if (id <= 0)
            {
                throw new ValidationException("Datos inválidos", "El ID debe ser mayor que cero");
            }

            try
            {
                var informationInfraction = await _informationInfractionData.GetByIdAsync(id);
                if (informationInfraction == null)
                {
                    throw new EntityNotFoundException("InformationInfraction", id);
                }

                informationInfraction.Numer_smldv = numer_smldv;
                informationInfraction.MinimumWage = minimumWage;
                informationInfraction.Value_smldv = value_smldv;
                informationInfraction.TotalValue = totalValue;

                var isUpdated = await _informationInfractionData.UpdateAsync(informationInfraction);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar el registro con ID {id}");
                }

                return MapToDTO(informationInfraction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la información de infracción con ID: {InformationInfractionId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la información de infracción con ID {id}", ex);
            }
        }

        /// <summary>
        /// Activa o desactiva un historial de pago por su ID.
        /// </summary>
        /// <param name="id">El ID del historial de pago.</param>
        /// <param name="isActive">Estado deseado: true para activar, false para desactivar.</param>
        /// <returns>El objeto PaymentHistoryDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra el historial de pago.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar el estado.</exception>
        public async Task<InformationInfractionDto> SetActiveStatusAsync(int id, bool isActive)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó cambiar el estado de un historial de pago con ID inválido.");
                throw new ValidationException("id", "El ID debe ser mayor que cero.");
            }

            try
            {
                var module = await _informationInfractionData.GetByIdAsync(id);
                if (module == null)
                {
                    throw new EntityNotFoundException("module", id);
                }

                // Actualizar el estado activo
                module.IsActive = isActive;

                var isUpdated = await _informationInfractionData.UpdateAsync(module);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo cambiar el estado del historial de pago con ID {id}.");
                }

                return MapToDTO(module);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar el estado del historial de pago con ID: {PaymentHistoryId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al cambiar el estado del historial de pago con ID {id}.", ex);
            }
        }


        /// <summary>
        /// Actualiza una infracción de información existente de manera asíncrona.
        /// </summary>
        /// <param name="informationInfractionDto">El objeto InformationInfractionDto con los datos actualizados de la infracción de información.</param>
        /// <returns>El objeto InformationInfractionDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si los datos son inválidos.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra la infracción de información.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar la infracción de información.</exception>
        public async Task<InformationInfractionDto> UpdateInformationInfractionAsync(InformationInfractionDto informationInfractionDto)
        {
            if (informationInfractionDto == null || informationInfractionDto.Id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar una infracción de información con datos inválidos o ID inválido.");
                throw new ValidationException("id", "El ID debe ser mayor que cero y los datos no pueden ser nulos.");
            }

            // Validar los datos del DTO
            ValidateInformationInfraction(informationInfractionDto);

            try
            {
                // Verificar si la infracción de información existe
                var existingInformationInfraction = await _informationInfractionData.GetByIdAsync(informationInfractionDto.Id);
                if (existingInformationInfraction == null)
                {
                    _logger.LogInformation("No se encontró ninguna infracción de información con ID: {InformationInfractionId}", informationInfractionDto.Id);
                    throw new EntityNotFoundException("InformationInfraction", informationInfractionDto.Id);
                }

                // Actualizar los datos de la infracción de información
                existingInformationInfraction.Numer_smldv = informationInfractionDto.Numer_smldv;
                existingInformationInfraction.MinimumWage = informationInfractionDto.MinimumWage;
                existingInformationInfraction.Value_smldv = informationInfractionDto.Value_smldv;
                existingInformationInfraction.TotalValue = informationInfractionDto.TotalValue;

                var isUpdated = await _informationInfractionData.UpdateAsync(existingInformationInfraction);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar la infracción de información con ID {informationInfractionDto.Id}.");
                }

                return MapToDTO(existingInformationInfraction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la infracción de información con ID: {InformationInfractionId}", informationInfractionDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la infracción de información con ID {informationInfractionDto.Id}.", ex);
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
                    TotalValue = informationInfractionDto.TotalValue,
                    IsActive = informationInfractionDto.IsActive
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
                TotalValue = informationInfraction.TotalValue,
                IsActive = informationInfraction.IsActive
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

