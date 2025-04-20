using Data;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Bussines
{
    /// <summary>
    /// Clase que maneja la lógica de negocio para el historial de pagos.
    /// </summary>
    public class PaymentHistoryBusiness
    {
        private readonly PaymentHistoryData _paymentHistoryData;
        private readonly ILogger<PaymentHistoryBusiness> _logger;

        /// <summary>
        /// Constructor de la clase PaymentHistoryBusiness.
        /// </summary>
        /// <param name="paymentHistoryData">Instancia de PaymentHistoryData para acceder a los datos del historial de pagos.</param>
        /// <param name="logger">Instancia de ILogger para el registro de logs.</param>
        public PaymentHistoryBusiness(PaymentHistoryData paymentHistoryData, ILogger<PaymentHistoryBusiness> logger)
        {
            _paymentHistoryData = paymentHistoryData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todo el historial de pagos de manera asíncrona.
        /// </summary>
        /// <returns>Una lista de objetos PaymentHistoryDto.</returns>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar el historial de pagos.</exception>
        public async Task<IEnumerable<PaymentHistoryDto>> GetAllPaymentHistoriesAsync()
        {
            try
            {
                var paymentHistories = await _paymentHistoryData.GetAllAsync();
                var visiblepaymentHistories = paymentHistories.Where(si => si.IsActive);
                return MapToDTOList(visiblepaymentHistories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todo el historial de pagos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar el historial de pagos", ex);
            }
        }

        /// <summary>
        /// Obtiene un historial de pago por su ID de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID del historial de pago.</param>
        /// <returns>Un objeto PaymentHistoryDto.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el historial de pago.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar el historial de pago.</exception>
        public async Task<PaymentHistoryDto> GetPaymentHistoryByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un historial de pago con ID inválido: {PaymentHistoryId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var paymentHistory = await _paymentHistoryData.GetByIdAsync(id);
                if (paymentHistory == null)
                {
                    _logger.LogInformation("No se encontró ningún historial de pago con ID: {PaymentHistoryId}", id);
                    throw new EntityNotFoundException("PaymentHistory", id);
                }

                return MapToDTO(paymentHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el historial de pago con ID: {PaymentHistoryId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el historial de pago con ID {id}", ex);
            }
        }

        /// <summary>
        /// Actualiza las propiedades Amount y PaymentDate de un historial de pago.
        /// </summary>
        /// <param name="id">El ID del historial de pago.</param>
        /// <param name="amount">El nuevo monto del pago.</param>
        /// <param name="paymentDate">La nueva fecha de pago.</param>
        /// <returns>El objeto PaymentHistoryDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si los datos son inválidos.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra el historial de pago.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar el historial de pago.</exception>
        public async Task<PaymentHistoryDto> UpdateAmountAndDateAsync(int id, decimal? amount, DateTime? paymentDate)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar un historial de pago con ID inválido.");
                throw new ValidationException("id", "El ID debe ser mayor que cero.");
            }

            try
            {
                var existingPaymentHistory = await _paymentHistoryData.GetByIdAsync(id);
                if (existingPaymentHistory == null)
                {
                    throw new EntityNotFoundException("PaymentHistory", id);
                }

                // Actualizar solo las propiedades proporcionadas
                if (amount.HasValue && amount.Value > 0)
                {
                    existingPaymentHistory.Amount = amount.Value;
                }

                if (paymentDate.HasValue && paymentDate.Value != default)
                {
                    existingPaymentHistory.PaymentDate = paymentDate.Value;
                }

                var isUpdated = await _paymentHistoryData.UpdateAsync(existingPaymentHistory);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar el historial de pago con ID {id}.");
                }

                return MapToDTO(existingPaymentHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el historial de pago con ID: {PaymentHistoryId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el historial de pago con ID {id}.", ex);
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
        public async Task<PaymentHistoryDto> SetActiveStatusAsync(int id, bool isActive)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó cambiar el estado de un historial de pago con ID inválido.");
                throw new ValidationException("id", "El ID debe ser mayor que cero.");
            }

            try
            {
                var paymentHistory = await _paymentHistoryData.GetByIdAsync(id);
                if (paymentHistory == null)
                {
                    throw new EntityNotFoundException("PaymentHistory", id);
                }

                // Actualizar el estado activo
                paymentHistory.IsActive = isActive;

                var isUpdated = await _paymentHistoryData.UpdateAsync(paymentHistory);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo cambiar el estado del historial de pago con ID {id}.");
                }

                return MapToDTO(paymentHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar el estado del historial de pago con ID: {PaymentHistoryId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al cambiar el estado del historial de pago con ID {id}.", ex);
            }
        }



        /// <summary>
        /// Elimina un historial de pago por su ID de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID del historial de pago a eliminar.</param>
        /// <returns>Un objeto PaymentHistoryDto del historial de pago eliminado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el historial de pago.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al eliminar el historial de pago.</exception>
        public async Task<PaymentHistoryDto> DeletePaymentHistoryAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un historial de pago con ID inválido: {PaymentHistoryId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                // Verificar si el historial de pago existe
                var paymentHistory = await _paymentHistoryData.GetByIdAsync(id);
                if (paymentHistory == null)
                {
                    _logger.LogInformation("No se encontró ningún historial de pago con ID: {PaymentHistoryId}", id);
                    throw new EntityNotFoundException("PaymentHistory", id);
                }

                // Intentar eliminar el historial de pago
                var isDeleted = await _paymentHistoryData.DeleteAsync(id);
                if (!isDeleted)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo eliminar el historial de pago con ID {id}");
                }

                // Devolver el objeto eliminado mapeado a DTO
                return MapToDTO(paymentHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el historial de pago con ID: {PaymentHistoryId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el historial de pago con ID {id}", ex);
            }
        }

        /// <summary>
        /// Actualiza un historial de pago existente de manera asíncrona.
        /// </summary>
        /// <param name="paymentHistoryDto">El objeto PaymentHistoryDto con los datos actualizados del historial de pago.</param>
        /// <returns>El objeto PaymentHistoryDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si los datos son inválidos.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra el historial de pago.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar el historial de pago.</exception>
        public async Task<PaymentHistoryDto> UpdatePaymentHistoryAsync(PaymentHistoryDto paymentHistoryDto)
        {
            if (paymentHistoryDto == null || paymentHistoryDto.Id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar un historial de pago con datos inválidos o ID inválido.");
                throw new ValidationException("id", "El ID debe ser mayor que cero y los datos no pueden ser nulos.");
            }

            // Validar los datos del DTO
            ValidatePaymentHistory(paymentHistoryDto);

            try
            {
                // Verificar si el historial de pago existe
                var existingPaymentHistory = await _paymentHistoryData.GetByIdAsync(paymentHistoryDto.Id);
                if (existingPaymentHistory == null)
                {
                    _logger.LogInformation("No se encontró ningún historial de pago con ID: {PaymentHistoryId}", paymentHistoryDto.Id);
                    throw new EntityNotFoundException("PaymentHistory", paymentHistoryDto.Id);
                }

                // Actualizar los datos del historial de pago
                existingPaymentHistory.UserId = paymentHistoryDto.UserId;
                existingPaymentHistory.Amount = paymentHistoryDto.Amount;
                existingPaymentHistory.PaymentDate = paymentHistoryDto.PaymentDate;

                var isUpdated = await _paymentHistoryData.UpdateAsync(existingPaymentHistory);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar el historial de pago con ID {paymentHistoryDto.Id}.");
                }

                return MapToDTO(existingPaymentHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el historial de pago con ID: {PaymentHistoryId}", paymentHistoryDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el historial de pago con ID {paymentHistoryDto.Id}.", ex);
            }
        }



        /// <summary>
        /// Crea un nuevo historial de pago de manera asíncrona.
        /// </summary>
        /// <param name="paymentHistoryDto">El objeto PaymentHistoryDto con los datos del historial de pago.</param>
        /// <returns>El objeto PaymentHistoryDto creado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando los datos del historial de pago son inválidos.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al crear el historial de pago.</exception>
        public async Task<PaymentHistoryDto> CreatePaymentHistoryAsync(PaymentHistoryDto paymentHistoryDto)
        {
            try
            {
                ValidatePaymentHistory(paymentHistoryDto);

                var paymentHistory = new PaymentHistory
                {
                    UserId = paymentHistoryDto.UserId,
                    Amount = paymentHistoryDto.Amount,
                    PaymentDate = paymentHistoryDto.PaymentDate,
                    CreatedAt = DateTime.UtcNow,
                    IsActive= paymentHistoryDto.IsActive
                };

                var createdPaymentHistory = await _paymentHistoryData.CreateAsync(paymentHistory);
                return MapToDTO(createdPaymentHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo historial de pago");
                throw new ExternalServiceException("Base de datos", "Error al crear el historial de pago", ex);
            }
        }

        /// <summary>
        /// Valida los datos del historial de pago.
        /// </summary>
        /// <param name="paymentHistoryDto">El objeto PaymentHistoryDto con los datos del historial de pago.</param>
        /// <exception cref="ValidationException">Lanzada cuando los datos del historial de pago son inválidos.</exception>
        private void ValidatePaymentHistory(PaymentHistoryDto paymentHistoryDto)
        {
            if (paymentHistoryDto == null)
            {
                throw new ValidationException("El objeto PaymentHistory no puede ser nulo");
            }

            if (paymentHistoryDto.UserId <= 0)
            {
                _logger.LogWarning("Se intentó crear un historial de pago con UserId inválido");
                throw new ValidationException("UserId", "El UserId debe ser mayor que cero");
            }

            if (paymentHistoryDto.Amount <= 0)
            {
                _logger.LogWarning("Se intentó crear un historial de pago con monto inválido");
                throw new ValidationException("Amount", "El monto debe ser mayor que cero");
            }

            if (paymentHistoryDto.PaymentDate == default)
            {
                _logger.LogWarning("Se intentó crear un historial de pago con fecha de pago inválida");
                throw new ValidationException("PaymentDate", "La fecha de pago no puede ser la predeterminada");
            }
        }

        /// <summary>
        /// Mapea un objeto PaymentHistory a PaymentHistoryDto.
        /// </summary>
        /// <param name="paymentHistory">El objeto PaymentHistory a mapear.</param>
        /// <returns>El objeto PaymentHistoryDto mapeado.</returns>
        private static PaymentHistoryDto MapToDTO(PaymentHistory paymentHistory)
        {
            return new PaymentHistoryDto
            {
                Id = paymentHistory.Id,
                UserId = paymentHistory.UserId,
                Amount = paymentHistory.Amount,
                PaymentDate = paymentHistory.PaymentDate,
                IsActive = paymentHistory.IsActive
            };
        }

        /// <summary>
        /// Mapea una lista de objetos PaymentHistory a una lista de PaymentHistoryDto.
        /// </summary>
        /// <param name="paymentHistories">La lista de objetos PaymentHistory a mapear.</param>
        /// <returns>La lista de objetos PaymentHistoryDto mapeados.</returns>
        private static IEnumerable<PaymentHistoryDto> MapToDTOList(IEnumerable<PaymentHistory> paymentHistories)
        {
            var paymentHistoriesDto = new List<PaymentHistoryDto>();
            foreach (var paymentHistory in paymentHistories)
            {
                paymentHistoriesDto.Add(MapToDTO(paymentHistory));
            }
            return paymentHistoriesDto;
        }
    }
}



