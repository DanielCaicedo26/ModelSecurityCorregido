using Data;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Bussines
{
    /// <summary>
    /// Clase que maneja la l�gica de negocio para los acuerdos de pago.
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
        /// Obtiene todos los acuerdos de pago de manera as�ncrona.
        /// </summary>
        /// <returns>Una lista de objetos PaymentAgreementDto.</returns>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar los acuerdos de pago.</exception>
        public async Task<IEnumerable<PaymentAgreementDto>> GetAllPaymentAgreementsAsync()
        {
            try
            {
                var paymentAgreements = await _paymentAgreementData.GetAllAsync();
                return MapToDTOList(paymentAgreements);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los acuerdos de pago");
                throw new ExternalServiceException("Base de datos", "Error al recuperar los acuerdos de pago", ex);
            }
        }

        /// <summary>
        /// Obtiene un acuerdo de pago por su ID de manera as�ncrona.
        /// </summary>
        /// <param name="id">El ID del acuerdo de pago.</param>
        /// <returns>Un objeto PaymentAgreementDto.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inv�lido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el acuerdo de pago.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar el acuerdo de pago.</exception>
        public async Task<PaymentAgreementDto> GetPaymentAgreementByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� obtener un acuerdo de pago con ID inv�lido: {PaymentAgreementId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var paymentAgreement = await _paymentAgreementData.GetByIdAsync(id);
                if (paymentAgreement == null)
                {
                    _logger.LogInformation("No se encontr� ning�n acuerdo de pago con ID: {PaymentAgreementId}", id);
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
        /// Crea un nuevo acuerdo de pago de manera as�ncrona.
        /// </summary>
        /// <param name="paymentAgreementDto">El objeto PaymentAgreementDto con los datos del acuerdo de pago.</param>
        /// <returns>El objeto PaymentAgreementDto creado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando los datos del acuerdo de pago son inv�lidos.</exception>
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
                    AgreementDescription = paymentAgreementDto.AgreementDescription
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
        /// <exception cref="ValidationException">Lanzada cuando los datos del acuerdo de pago son inv�lidos.</exception>
        private void ValidatePaymentAgreement(PaymentAgreementDto paymentAgreementDto)
        {
            if (paymentAgreementDto == null)
            {
                throw new ValidationException("El objeto PaymentAgreement no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(paymentAgreementDto.Address))
            {
                _logger.LogWarning("Se intent� crear un acuerdo de pago con direcci�n vac�a");
                throw new ValidationException("Address", "La direcci�n no puede estar vac�a");
            }

            if (paymentAgreementDto.FinanceAmount <= 0)
            {
                _logger.LogWarning("Se intent� crear un acuerdo de pago con monto financiero inv�lido");
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
                AgreementDescription = paymentAgreement.AgreementDescription
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



