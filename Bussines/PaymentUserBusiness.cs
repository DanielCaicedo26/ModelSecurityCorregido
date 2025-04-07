using Data;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Bussines
{
    /// <summary>
    /// Clase que maneja la lógica de negocio para los pagos de usuarios.
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
        /// Obtiene todos los pagos de usuarios de manera asíncrona.
        /// </summary>
        /// <returns>Una lista de objetos PaymentUserDto.</returns>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar los pagos de usuarios.</exception>
        public async Task<IEnumerable<PaymentUserDto>> GetAllPaymentUsersAsync()
        {
            try
            {
                var paymentUsers = await _paymentUserData.GetAllAsync();
                return MapToDTOList(paymentUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los pagos de usuarios");
                throw new ExternalServiceException("Base de datos", "Error al recuperar los pagos de usuarios", ex);
            }
        }

        /// <summary>
        /// Obtiene un pago de usuario por su ID de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID del pago de usuario.</param>
        /// <returns>Un objeto PaymentUserDto.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el pago de usuario.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar el pago de usuario.</exception>
        public async Task<PaymentUserDto> GetPaymentUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un pago de usuario con ID inválido: {PaymentUserId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var paymentUser = await _paymentUserData.GetByIdAsync(id);
                if (paymentUser == null)
                {
                    _logger.LogInformation("No se encontró ningún pago de usuario con ID: {PaymentUserId}", id);
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
        /// Crea un nuevo pago de usuario de manera asíncrona.
        /// </summary>
        /// <param name="paymentUserDto">El objeto PaymentUserDto con los datos del pago de usuario.</param>
        /// <returns>El objeto PaymentUserDto creado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando los datos del pago de usuario son inválidos.</exception>
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
                    CreatedAt = DateTime.UtcNow
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
        /// <exception cref="ValidationException">Lanzada cuando los datos del pago de usuario son inválidos.</exception>
        private void ValidatePaymentUser(PaymentUserDto paymentUserDto)
        {
            if (paymentUserDto == null)
            {
                throw new ValidationException("El objeto PaymentUser no puede ser nulo");
            }

            if (paymentUserDto.PersonId <= 0)
            {
                _logger.LogWarning("Se intentó crear un pago de usuario con PersonId inválido");
                throw new ValidationException("PersonId", "El PersonId debe ser mayor que cero");
            }

            if (paymentUserDto.Amount <= 0)
            {
                _logger.LogWarning("Se intentó crear un pago de usuario con monto inválido");
                throw new ValidationException("Amount", "El monto debe ser mayor que cero");
            }

            if (paymentUserDto.PaymentDate == default)
            {
                _logger.LogWarning("Se intentó crear un pago de usuario con fecha de pago inválida");
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
                PaymentDate = paymentUser.PaymentDate
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



