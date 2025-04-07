using Data;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Bussines
{
    /// <summary>
    /// Clase que maneja la lógica de negocio para las facturas.
    /// </summary>
    public class BillBusiness
    {
        private readonly BillData _billData;
        private readonly ILogger<BillBusiness> _logger;

        /// <summary>
        /// Constructor de la clase BillBusiness.
        /// </summary>
        /// <param name="billData">Instancia de BillData para acceder a los datos de las facturas.</param>
        /// <param name="logger">Instancia de ILogger para el registro de logs.</param>
        public BillBusiness(BillData billData, ILogger<BillBusiness> logger)
        {
            _billData = billData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las facturas de manera asíncrona.
        /// </summary>
        /// <returns>Una lista de objetos BillDto.</returns>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar las facturas.</exception>
        public async Task<IEnumerable<BillDto>> GetAllBillsAsync()
        {
            try
            {
                var bills = await _billData.GetAllAsync();
                return MapToDTOList(bills);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las facturas");
                throw new ExternalServiceException("Base de datos", "Error al recuperar las facturas", ex);
            }
        }

        /// <summary>
        /// Obtiene una factura por su ID de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID de la factura.</param>
        /// <returns>Un objeto BillDto.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra la factura.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar la factura.</exception>
        public async Task<BillDto> GetBillByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una factura con ID inválido: {BillId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var bill = await _billData.GetByIdAsync(id);
                if (bill == null)
                {
                    _logger.LogInformation("No se encontró ninguna factura con ID: {BillId}", id);
                    throw new EntityNotFoundException("Bill", id);
                }

                return MapToDTO(bill);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la factura con ID: {BillId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la factura con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea una nueva factura de manera asíncrona.
        /// </summary>
        /// <param name="billDto">El objeto BillDto con los datos de la factura.</param>
        /// <returns>El objeto BillDto creado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando los datos de la factura son inválidos.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al crear la factura.</exception>
        public async Task<BillDto> CreateBillAsync(BillDto billDto)
        {
            try
            {
                ValidateBill(billDto);

                var bill = new Bill
                {
                    Barcode = billDto.Barcode,
                    IssueDate = billDto.IssueDate,
                    ExpirationDate = billDto.ExpirationDate,
                    TotalValue = billDto.TotalValue,
                    State = billDto.State,

                };

                var createdBill = await _billData.CreateAsync(bill);
                return MapToDTO(createdBill);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear una nueva factura");
                throw new ExternalServiceException("Base de datos", "Error al crear la factura", ex);
            }
        }

        /// <summary>
        /// Valida los datos de la factura.
        /// </summary>
        /// <param name="billDto">El objeto BillDto con los datos de la factura.</param>
        /// <exception cref="ValidationException">Lanzada cuando los datos de la factura son inválidos.</exception>
        private void ValidateBill(BillDto billDto)
        {
            if (billDto == null)
            {
                throw new ValidationException("El objeto Bill no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(billDto.Barcode))
            {
                _logger.LogWarning("Se intentó crear una factura con código de barras vacío");
                throw new ValidationException("Barcode", "El código de barras no puede estar vacío");
            }

            if (billDto.TotalValue <= 0)
            {
                _logger.LogWarning("Se intentó crear una factura con valor total inválido");
                throw new ValidationException("TotalValue", "El valor total debe ser mayor que cero");
            }

            if (billDto.IssueDate == default)
            {
                _logger.LogWarning("Se intentó crear una factura con fecha de emisión inválida");
                throw new ValidationException("IssueDate", "La fecha de emisión no puede ser la predeterminada");
            }

            if (billDto.ExpirationDate == default)
            {
                _logger.LogWarning("Se intentó crear una factura con fecha de vencimiento inválida");
                throw new ValidationException("ExpirationDate", "La fecha de vencimiento no puede ser la predeterminada");
            }
        }

        /// <summary>
        /// Mapea un objeto Bill a BillDto.
        /// </summary>
        /// <param name="bill">El objeto Bill a mapear.</param>
        /// <returns>El objeto BillDto mapeado.</returns>
        private static BillDto MapToDTO(Bill bill)
        {
            return new BillDto
            {
                Id = bill.Id,
                Barcode = bill.Barcode,
                IssueDate = bill.IssueDate,
                ExpirationDate = bill.ExpirationDate,
                TotalValue = bill.TotalValue,
                State = bill.State
            };
        }

        /// <summary>
        /// Mapea una lista de objetos Bill a una lista de BillDto.
        /// </summary>
        /// <param name="bills">La lista de objetos Bill a mapear.</param>
        /// <returns>La lista de objetos BillDto mapeados.</returns>
        private static IEnumerable<BillDto> MapToDTOList(IEnumerable<Bill> bills)
        {
            var billsDto = new List<BillDto>();
            foreach (var bill in bills)
            {
                billsDto.Add(MapToDTO(bill));
            }
            return billsDto;
        }
    }
}
