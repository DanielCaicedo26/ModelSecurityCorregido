using Data;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Bussines
{
    /// <summary>
    /// Clase que maneja la l�gica de negocio para los pagos de usuarios.
    /// </summary>
    public class PaymentUserBusiness
    {
        private readonly PaymentUserData _paymentUserData;
        private readonly ILogger<PaymentUserBusiness> _logger;

        /// <summary>
        /// Constructor de la clase PaymentUserBusiness.
        /// </summary>
        /// <param name="paymentUserData">Instancia de PaymentUserData para acceder a los datos de los pagos de usuarios.</param>
        /// <param name="logger">Instancia de ILogger para el registro de logs.</param>
        public PaymentUserBusiness(PaymentUserData paymentUserData, ILogger<PaymentUserBusiness> logger)
        {
            _paymentUserData = paymentUserData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los pagos de usuarios de manera as�ncrona.
        /// </summary>
        /// <returns>Una lista de objetos PaymentUserDto.</returns>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar los pagos de usuarios.</exception>
        public async Task<IEnumerable<PaymentUserDto>> GetAllPaymentUsersAsync()
        {
            try
            {
                var paymentUsers = await _paymentUserData.GetAllAsync();
                var visiblepaymentUsers = paymentUsers.Where(si => si.IsActive);
                return MapToDTOList(visiblepaymentUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los pagos de usuarios");
                throw new ExternalServiceException("Base de datos", "Error al recuperar los pagos de usuarios", ex);
            }
        }

        /// <summary>
        /// Obtiene un pago de usuario por su ID de manera as�ncrona.
        /// </summary>
        /// <param name="id">El ID del pago de usuario.</param>
        /// <returns>Un objeto PaymentUserDto.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inv�lido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el pago de usuario.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar el pago de usuario.</exception>
        public async Task<PaymentUserDto> GetPaymentUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� obtener un pago de usuario con ID inv�lido: {PaymentUserId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var paymentUser = await _paymentUserData.GetByIdAsync(id);
                if (paymentUser == null)
                {
                    _logger.LogInformation("No se encontr� ning�n pago de usuario con ID: {PaymentUserId}", id);
                    throw new EntityNotFoundException("PaymentUser", id);
                }

                return MapToDTO(paymentUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el pago de usuario con ID: {PaymentUserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el pago de usuario con ID {id}", ex);
            }
        }

        /// <summary>
        /// Elimina un pago de usuario por su ID de manera as�ncrona.
        /// </summary>
        /// <param name="id">El ID del pago de usuario a eliminar.</param>
        /// <returns>Un objeto PaymentUserDto del pago de usuario eliminado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inv�lido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el pago de usuario.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al eliminar el pago de usuario.</exception>
        public async Task<PaymentUserDto> DeletePaymentUserAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� eliminar un pago de usuario con ID inv�lido: {PaymentUserId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                // Verificar si el pago de usuario existe
                var paymentUser = await _paymentUserData.GetByIdAsync(id);
                if (paymentUser == null)
                {
                    _logger.LogInformation("No se encontr� ning�n pago de usuario con ID: {PaymentUserId}", id);
                    throw new EntityNotFoundException("PaymentUser", id);
                }

                // Intentar eliminar el pago de usuario
                var isDeleted = await _paymentUserData.DeleteAsync(id);
                if (!isDeleted)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo eliminar el pago de usuario con ID {id}");
                }

                // Devolver el objeto eliminado mapeado a DTO
                return MapToDTO(paymentUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el pago de usuario con ID: {PaymentUserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el pago de usuario con ID {id}", ex);
            }
        }



        /// <summary>
        /// Actualiza los datos de un pago de usuario.
        /// </summary>
        /// <param name="paymentUserDto">El objeto PaymentUserDto con los datos actualizados del pago de usuario.</param>
        /// <returns>El objeto PaymentUserDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si los datos son inv�lidos.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra el pago de usuario.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar el pago de usuario.</exception>
        public async Task<PaymentUserDto> UpdatePaymentUserAsync(PaymentUserDto paymentUserDto)
        {
            if (paymentUserDto == null || paymentUserDto.Id <= 0)
            {
                _logger.LogWarning("Se intent� actualizar un pago de usuario con datos inv�lidos o ID inv�lido.");
                throw new ValidationException("id", "El ID debe ser mayor que cero y los datos no pueden ser nulos.");
            }

            // Validar los datos del DTO
            ValidatePaymentUser(paymentUserDto);

            try
            {
                // Verificar si el pago de usuario existe
                var existingPaymentUser = await _paymentUserData.GetByIdAsync(paymentUserDto.Id);
                if (existingPaymentUser == null)
                {
                    _logger.LogInformation("No se encontr� ning�n pago de usuario con ID: {PaymentUserId}", paymentUserDto.Id);
                    throw new EntityNotFoundException("PaymentUser", paymentUserDto.Id);
                }

                // Actualizar los datos del pago de usuario
                existingPaymentUser.PersonId = paymentUserDto.PersonId;
                existingPaymentUser.Amount = paymentUserDto.Amount;
                existingPaymentUser.PaymentDate = paymentUserDto.PaymentDate;

                var isUpdated = await _paymentUserData.UpdateAsync(existingPaymentUser);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar el pago de usuario con ID {paymentUserDto.Id}.");
                }

                return MapToDTO(existingPaymentUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el pago de usuario con ID: {PaymentUserId}", paymentUserDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el pago de usuario con ID {paymentUserDto.Id}.", ex);
            }
        }
        /// <summary>
        /// Actualiza propiedades espec�ficas de un pago de usuario.
        /// </summary>
        /// <param name="id">El ID del pago de usuario.</param>
        /// <param name="amount">El nuevo monto del pago.</param>
        /// <param name="paymentDate">La nueva fecha de pago.</param>
        /// <returns>El objeto PaymentUserDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si los datos son inv�lidos.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra el pago de usuario.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar el pago de usuario.</exception>
        public async Task<PaymentUserDto> UpdatePartialAsync(int id, decimal? amount, DateTime? paymentDate)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� actualizar un pago de usuario con ID inv�lido.");
                throw new ValidationException("id", "El ID debe ser mayor que cero.");
            }

            try
            {
                var existingPaymentUser = await _paymentUserData.GetByIdAsync(id);
                if (existingPaymentUser == null)
                {
                    throw new EntityNotFoundException("PaymentUser", id);
                }

                // Actualizar solo las propiedades proporcionadas
                if (amount.HasValue && amount.Value > 0)
                {
                    existingPaymentUser.Amount = amount.Value;
                }

                if (paymentDate.HasValue && paymentDate.Value != default)
                {
                    existingPaymentUser.PaymentDate = paymentDate.Value;
                }

                var isUpdated = await _paymentUserData.UpdateAsync(existingPaymentUser);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar el pago de usuario con ID {id}.");
                }

                return MapToDTO(existingPaymentUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el pago de usuario con ID: {PaymentUserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el pago de usuario con ID {id}.", ex);
            }
        }

        /// <summary>
        /// Activa o desactiva un pago de usuario por su ID.
        /// </summary>
        /// <param name="id">El ID del pago de usuario.</param>
        /// <param name="isActive">Estado deseado: true para activar, false para desactivar.</param>
        /// <returns>El objeto PaymentUserDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si el ID es inv�lido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra el pago de usuario.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar el estado.</exception>
        public async Task<PaymentUserDto> SetActiveStatusAsync(int id, bool isActive)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� cambiar el estado de un pago de usuario con ID inv�lido.");
                throw new ValidationException("id", "El ID debe ser mayor que cero.");
            }

            try
            {
                var paymentUser = await _paymentUserData.GetByIdAsync(id);
                if (paymentUser == null)
                {
                    throw new EntityNotFoundException("PaymentUser", id);
                }

                // Actualizar el estado activo
                paymentUser.IsActive = isActive;

                var isUpdated = await _paymentUserData.UpdateAsync(paymentUser);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo cambiar el estado del pago de usuario con ID {id}.");
                }

                return MapToDTO(paymentUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar el estado del pago de usuario con ID: {PaymentUserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al cambiar el estado del pago de usuario con ID {id}.", ex);
            }
        }






        /// <summary>
        /// Crea un nuevo pago de usuario de manera as�ncrona.
        /// </summary>
        /// <param name="paymentUserDto">El objeto PaymentUserDto con los datos del pago de usuario.</param>
        /// <returns>El objeto PaymentUserDto creado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando los datos del pago de usuario son inv�lidos.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al crear el pago de usuario.</exception>
        public async Task<PaymentUserDto> CreatePaymentUserAsync(PaymentUserDto paymentUserDto)
        {
            try
            {
                ValidatePaymentUser(paymentUserDto);

                var paymentUser = new PaymentUser
                {
                    PersonId = paymentUserDto.PersonId,
                    Amount = paymentUserDto.Amount,
                    PaymentDate = paymentUserDto.PaymentDate,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = paymentUserDto.IsActive
                };

                var createdPaymentUser = await _paymentUserData.CreateAsync(paymentUser);
                return MapToDTO(createdPaymentUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo pago de usuario");
                throw new ExternalServiceException("Base de datos", "Error al crear el pago de usuario", ex);
            }
        }

        /// <summary>
        /// Valida los datos del pago de usuario.
        /// </summary>
        /// <param name="paymentUserDto">El objeto PaymentUserDto con los datos del pago de usuario.</param>
        /// <exception cref="ValidationException">Lanzada cuando los datos del pago de usuario son inv�lidos.</exception>
        private void ValidatePaymentUser(PaymentUserDto paymentUserDto)
        {
            if (paymentUserDto == null)
            {
                throw new ValidationException("El objeto PaymentUser no puede ser nulo");
            }

            if (paymentUserDto.PersonId <= 0)
            {
                _logger.LogWarning("Se intent� crear un pago de usuario con PersonId inv�lido");
                throw new ValidationException("PersonId", "El PersonId debe ser mayor que cero");
            }

            if (paymentUserDto.Amount <= 0)
            {
                _logger.LogWarning("Se intent� crear un pago de usuario con monto inv�lido");
                throw new ValidationException("Amount", "El monto debe ser mayor que cero");
            }

            if (paymentUserDto.PaymentDate == default)
            {
                _logger.LogWarning("Se intent� crear un pago de usuario con fecha de pago inv�lida");
                throw new ValidationException("PaymentDate", "La fecha de pago no puede ser la predeterminada");
            }
        }

        /// <summary>
        /// Mapea un objeto PaymentUser a PaymentUserDto.
        /// </summary>
        /// <param name="paymentUser">El objeto PaymentUser a mapear.</param>
        /// <returns>El objeto PaymentUserDto mapeado.</returns>
        private static PaymentUserDto MapToDTO(PaymentUser paymentUser)
        {
            return new PaymentUserDto
            {
                Id = paymentUser.Id,
                PersonId = paymentUser.PersonId,
                Amount = paymentUser.Amount,
                PaymentDate = paymentUser.PaymentDate,
                IsActive = paymentUser.IsActive
            };
        }

        /// <summary>
        /// Mapea una lista de objetos PaymentUser a una lista de PaymentUserDto.
        /// </summary>
        /// <param name="paymentUsers">La lista de objetos PaymentUser a mapear.</param>
        /// <returns>La lista de objetos PaymentUserDto mapeados.</returns>
        private static IEnumerable<PaymentUserDto> MapToDTOList(IEnumerable<PaymentUser> paymentUsers)
        {
            var paymentUsersDto = new List<PaymentUserDto>();
            foreach (var paymentUser in paymentUsers)
            {
                paymentUsersDto.Add(MapToDTO(paymentUser));
            }
            return paymentUsersDto;
        }
    }
}



