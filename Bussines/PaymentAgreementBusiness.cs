using Data;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Bussines
{
    /// <summary>
    /// Clase que maneja la lógica de negocio para los acuerdos de pago.
    /// </summary>
    public class PaymentAgreementBusiness
    {
        private readonly PaymentAgreementData _paymentAgreementData;
        private readonly ILogger<PaymentAgreementBusiness> _logger;

        /// <summary>
        /// Constructor de la clase PaymentAgreementBusiness.
        /// </summary>
        /// <param name="paymentAgreementData">Instancia de PaymentAgreementData para acceder a los datos de los acuerdos de pago.</param>
        /// <param name="logger">Instancia de ILogger para el registro de logs.</param>
        public PaymentAgreementBusiness(PaymentAgreementData paymentAgreementData, ILogger<PaymentAgreementBusiness> logger)
        {
            _paymentAgreementData = paymentAgreementData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los acuerdos de pago de manera asíncrona.
        /// </summary>
        /// <returns>Una lista de objetos PaymentAgreementDto.</returns>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar los acuerdos de pago.</exception>
        public async Task<IEnumerable<PaymentAgreementDto>> GetAllPaymentAgreementsAsync()
        {
            try
            {
                var paymentAgreements = await _paymentAgreementData.GetAllAsync();
                var visiblepaymentAgreements= paymentAgreements.Where(si => si.IsActive);
                return MapToDTOList(visiblepaymentAgreements);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los acuerdos de pago");
                throw new ExternalServiceException("Base de datos", "Error al recuperar los acuerdos de pago", ex);
            }
        }

        /// <summary>
        /// Obtiene un acuerdo de pago por su ID de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID del acuerdo de pago.</param>
        /// <returns>Un objeto PaymentAgreementDto.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el acuerdo de pago.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar el acuerdo de pago.</exception>
        public async Task<PaymentAgreementDto> GetPaymentAgreementByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un acuerdo de pago con ID inválido: {PaymentAgreementId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var paymentAgreement = await _paymentAgreementData.GetByIdAsync(id);
                if (paymentAgreement == null)
                {
                    _logger.LogInformation("No se encontró ningún acuerdo de pago con ID: {PaymentAgreementId}", id);
                    throw new EntityNotFoundException("PaymentAgreement", id);
                }

                return MapToDTO(paymentAgreement);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el acuerdo de pago con ID: {PaymentAgreementId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el acuerdo de pago con ID {id}", ex);
            }
        }

        /// <summary>
        /// Elimina un acuerdo de pago por su ID de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID del acuerdo de pago a eliminar.</param>
        /// <returns>Un objeto PaymentAgreementDto del acuerdo de pago eliminado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el acuerdo de pago.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al eliminar el acuerdo de pago.</exception>
        public async Task<PaymentAgreementDto> DeletePaymentAgreementAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un acuerdo de pago con ID inválido: {PaymentAgreementId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                // Verificar si el acuerdo de pago existe
                var paymentAgreement = await _paymentAgreementData.GetByIdAsync(id);
                if (paymentAgreement == null)
                {
                    _logger.LogInformation("No se encontró ningún acuerdo de pago con ID: {PaymentAgreementId}", id);
                    throw new EntityNotFoundException("PaymentAgreement", id);
                }

                // Intentar eliminar el acuerdo de pago
                var isDeleted = await _paymentAgreementData.DeleteAsync(id);
                if (!isDeleted)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo eliminar el acuerdo de pago con ID {id}");
                }

                // Devolver el objeto eliminado mapeado a DTO
                return MapToDTO(paymentAgreement);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el acuerdo de pago con ID: {PaymentAgreementId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el acuerdo de pago con ID {id}", ex);
            }
        }

        /// <summary>
        /// Actualiza propiedades específicas de un acuerdo de pago.
        /// </summary>
        /// <param name="id">El ID del acuerdo de pago.</param>
        /// <param name="address">La nueva dirección del acuerdo de pago.</param>
        /// <param name="neighborhood">El nuevo vecindario del acuerdo de pago.</param>
        /// <param name="financeAmount">El nuevo monto financiero del acuerdo de pago.</param>
        /// <param name="agreementDescription">La nueva descripción del acuerdo de pago.</param>
        /// <returns>El objeto PaymentAgreementDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si los datos son inválidos.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra el acuerdo de pago.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar el acuerdo de pago.</exception>
        public async Task<PaymentAgreementDto> UpdatePartialAsync(
            int id,
            string? address,
            string? neighborhood,
            decimal? financeAmount,
            string? agreementDescription
        )
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar un acuerdo de pago con ID inválido.");
                throw new ValidationException("id", "El ID debe ser mayor que cero.");
            }

            try
            {
                var existingPaymentAgreement = await _paymentAgreementData.GetByIdAsync(id);
                if (existingPaymentAgreement == null)
                {
                    throw new EntityNotFoundException("PaymentAgreement", id);
                }

                // Actualizar solo las propiedades proporcionadas
                if (!string.IsNullOrWhiteSpace(address))
                {
                    existingPaymentAgreement.Address = address;
                }

                if (!string.IsNullOrWhiteSpace(neighborhood))
                {
                    existingPaymentAgreement.Neighborhood = neighborhood;
                }

                if (financeAmount.HasValue && financeAmount.Value > 0)
                {
                    existingPaymentAgreement.FinanceAmount = financeAmount.Value;
                }

                if (!string.IsNullOrWhiteSpace(agreementDescription))
                {
                    existingPaymentAgreement.AgreementDescription = agreementDescription;
                }

                var isUpdated = await _paymentAgreementData.UpdateAsync(existingPaymentAgreement);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar el acuerdo de pago con ID {id}.");
                }

                return MapToDTO(existingPaymentAgreement);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el acuerdo de pago con ID: {PaymentAgreementId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el acuerdo de pago con ID {id}.", ex);
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
        public async Task<PaymentAgreementDto> SetActiveStatusAsync(int id, bool isActive)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó cambiar el estado de un historial de pago con ID inválido.");
                throw new ValidationException("id", "El ID debe ser mayor que cero.");
            }

            try
            {
                var paymentAgreement = await _paymentAgreementData.GetByIdAsync(id);
                if (paymentAgreement == null)
                {
                    throw new EntityNotFoundException("PaymentHistory", id);
                }

                // Actualizar el estado activo
                paymentAgreement.IsActive = isActive;

                var isUpdated = await _paymentAgreementData.UpdateAsync(paymentAgreement);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo cambiar el estado del historial de pago con ID {id}.");
                }

                return MapToDTO(paymentAgreement);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar el estado del historial de pago con ID: {PaymentHistoryId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al cambiar el estado del historial de pago con ID {id}.", ex);
            }
        }


        /// <summary>
        /// Actualiza un acuerdo de pago existente de manera asíncrona.
        /// </summary>
        /// <param name="paymentAgreementDto">El objeto PaymentAgreementDto con los datos actualizados del acuerdo de pago.</param>
        /// <returns>El objeto PaymentAgreementDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si los datos son inválidos.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra el acuerdo de pago.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar el acuerdo de pago.</exception>
        public async Task<PaymentAgreementDto> UpdatePaymentAgreementAsync(PaymentAgreementDto paymentAgreementDto)
        {
            if (paymentAgreementDto == null || paymentAgreementDto.Id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar un acuerdo de pago con datos inválidos o ID inválido.");
                throw new ValidationException("id", "El ID debe ser mayor que cero y los datos no pueden ser nulos.");
            }

            // Validar los datos del DTO
            ValidatePaymentAgreement(paymentAgreementDto);

            try
            {
                // Verificar si el acuerdo de pago existe
                var existingPaymentAgreement = await _paymentAgreementData.GetByIdAsync(paymentAgreementDto.Id);
                if (existingPaymentAgreement == null)
                {
                    _logger.LogInformation("No se encontró ningún acuerdo de pago con ID: {PaymentAgreementId}", paymentAgreementDto.Id);
                    throw new EntityNotFoundException("PaymentAgreement", paymentAgreementDto.Id);
                }

                // Actualizar los datos del acuerdo de pago
                existingPaymentAgreement.Address = paymentAgreementDto.Address;
                existingPaymentAgreement.Neighborhood = paymentAgreementDto.Neighborhood;
                existingPaymentAgreement.FinanceAmount = paymentAgreementDto.FinanceAmount;
                existingPaymentAgreement.AgreementDescription = paymentAgreementDto.AgreementDescription;

                var isUpdated = await _paymentAgreementData.UpdateAsync(existingPaymentAgreement);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar el acuerdo de pago con ID {paymentAgreementDto.Id}.");
                }

                return MapToDTO(existingPaymentAgreement);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el acuerdo de pago con ID: {PaymentAgreementId}", paymentAgreementDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el acuerdo de pago con ID {paymentAgreementDto.Id}.", ex);
            }
        }




        /// <summary>
        /// Crea un nuevo acuerdo de pago de manera asíncrona.
        /// </summary>
        /// <param name="paymentAgreementDto">El objeto PaymentAgreementDto con los datos del acuerdo de pago.</param>
        /// <returns>El objeto PaymentAgreementDto creado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando los datos del acuerdo de pago son inválidos.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al crear el acuerdo de pago.</exception>
        public async Task<PaymentAgreementDto> CreatePaymentAgreementAsync(PaymentAgreementDto paymentAgreementDto)
        {
            try
            {
                ValidatePaymentAgreement(paymentAgreementDto);

                var paymentAgreement = new PaymentAgreement
                {
                    Address = paymentAgreementDto.Address,
                    Neighborhood = paymentAgreementDto.Neighborhood,
                    FinanceAmount = paymentAgreementDto.FinanceAmount,
                    AgreementDescription = paymentAgreementDto.AgreementDescription,
                    IsActive = paymentAgreementDto.IsActive

                };

                var createdPaymentAgreement = await _paymentAgreementData.CreateAsync(paymentAgreement);
                return MapToDTO(createdPaymentAgreement);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo acuerdo de pago");
                throw new ExternalServiceException("Base de datos", "Error al crear el acuerdo de pago", ex);
            }
        }

        /// <summary>
        /// Valida los datos del acuerdo de pago.
        /// </summary>
        /// <param name="paymentAgreementDto">El objeto PaymentAgreementDto con los datos del acuerdo de pago.</param>
        /// <exception cref="ValidationException">Lanzada cuando los datos del acuerdo de pago son inválidos.</exception>
        private void ValidatePaymentAgreement(PaymentAgreementDto paymentAgreementDto)
        {
            if (paymentAgreementDto == null)
            {
                throw new ValidationException("El objeto PaymentAgreement no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(paymentAgreementDto.Address))
            {
                _logger.LogWarning("Se intentó crear un acuerdo de pago con dirección vacía");
                throw new ValidationException("Address", "La dirección no puede estar vacía");
            }

            if (paymentAgreementDto.FinanceAmount <= 0)
            {
                _logger.LogWarning("Se intentó crear un acuerdo de pago con monto financiero inválido");
                throw new ValidationException("FinanceAmount", "El monto financiero debe ser mayor que cero");
            }
        }

        /// <summary>
        /// Mapea un objeto PaymentAgreement a PaymentAgreementDto.
        /// </summary>
        /// <param name="paymentAgreement">El objeto PaymentAgreement a mapear.</param>
        /// <returns>El objeto PaymentAgreementDto mapeado.</returns>
        private static PaymentAgreementDto MapToDTO(PaymentAgreement paymentAgreement)
        {
            return new PaymentAgreementDto
            {
                Id = paymentAgreement.Id,
                Address = paymentAgreement.Address,
                Neighborhood = paymentAgreement.Neighborhood,
                FinanceAmount = paymentAgreement.FinanceAmount,
                AgreementDescription = paymentAgreement.AgreementDescription,
                IsActive = paymentAgreement.IsActive

            };
        }

        /// <summary>
        /// Mapea una lista de objetos PaymentAgreement a una lista de PaymentAgreementDto.
        /// </summary>
        /// <param name="paymentAgreements">La lista de objetos PaymentAgreement a mapear.</param>
        /// <returns>La lista de objetos PaymentAgreementDto mapeados.</returns>
        private static IEnumerable<PaymentAgreementDto> MapToDTOList(IEnumerable<PaymentAgreement> paymentAgreements)
        {
            var paymentAgreementsDto = new List<PaymentAgreementDto>();
            foreach (var paymentAgreement in paymentAgreements)
            {
                paymentAgreementsDto.Add(MapToDTO(paymentAgreement));
            }
            return paymentAgreementsDto;
        }
    }
}



