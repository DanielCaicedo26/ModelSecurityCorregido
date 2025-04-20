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
                var visiblebills = bills.Where(n => n.IsActive);
                return MapToDTOList(visiblebills);
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
        /// Elimina una factura por su ID de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID de la factura a eliminar.</param>
        /// <returns>Un objeto BillDto de la factura eliminada.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra la factura.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al eliminar la factura.</exception>
        public async Task<BillDto> DeleteBillAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar una factura con ID inválido: {BillId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                // Verificar si la factura existe
                var bill = await _billData.GetByIdAsync(id);
                if (bill == null)
                {
                    _logger.LogInformation("No se encontró ninguna factura con ID: {BillId}", id);
                    throw new EntityNotFoundException("Bill", id);
                }

                // Intentar eliminar la factura
                var isDeleted = await _billData.DeleteAsync(id);
                if (!isDeleted)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo eliminar la factura con ID {id}");
                }

                // Devolver el objeto eliminado mapeado a DTO
                return MapToDTO(bill);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la factura con ID: {BillId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar la factura con ID {id}", ex);
            }
        }

        /// <summary>
        /// Actualiza una factura existente de manera asíncrona.
        /// </summary>
        /// <param name="billDto">El objeto BillDto con los datos actualizados de la factura.</param>
        /// <returns>El objeto BillDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si los datos son inválidos.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra la factura.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar la factura.</exception>
        public async Task<BillDto> UpdateBillAsync(BillDto billDto)
        {
            if (billDto == null || billDto.Id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar una factura con datos inválidos o ID inválido.");
                throw new ValidationException("id", "El ID debe ser mayor que cero y los datos no pueden ser nulos.");
            }

            // Validar los datos del DTO
            ValidateBill(billDto);

            try
            {
                // Verificar si la factura existe
                var existingBill = await _billData.GetByIdAsync(billDto.Id);
                if (existingBill == null)
                {
                    _logger.LogInformation("No se encontró ninguna factura con ID: {BillId}", billDto.Id);
                    throw new EntityNotFoundException("Bill", billDto.Id);
                }

                // Actualizar los datos de la factura
                existingBill.Barcode = billDto.Barcode;
                existingBill.IssueDate = billDto.IssueDate;
                existingBill.ExpirationDate = billDto.ExpirationDate;
                existingBill.TotalValue = billDto.TotalValue;
                existingBill.State = billDto.State;

                var isUpdated = await _billData.UpdateAsync(existingBill);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar la factura con ID {billDto.Id}.");
                }

                return MapToDTO(existingBill);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la factura con ID: {BillId}", billDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la factura con ID {billDto.Id}.", ex);
            }
        }

        public async Task<BillDto> Update(int id, string barcode, DateTime issueDate, DateTime expirationDate, decimal totalValue, string? state)
        {
            if (id <= 0 || string.IsNullOrWhiteSpace(barcode))
            {
                throw new ValidationException("Datos inválidos", "El ID debe ser mayor que cero y el código de barras no puede estar vacío.");
            }

            try
            {
                var bill = await _billData.GetByIdAsync(id);
                if (bill == null)
                {
                    throw new EntityNotFoundException("Bill", id);
                }

                bill.Barcode = barcode;
                bill.IssueDate = issueDate;
                bill.ExpirationDate = expirationDate;
                bill.TotalValue = totalValue;
                bill.State = state;
                

                var isUpdated = await _billData.UpdateAsync(bill);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar la factura con ID {id}");
                }

                return MapToDTO(bill);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la factura con ID: {BillId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la factura con ID {id}", ex);
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
        public async Task<BillDto> SetActiveStatusAsync(int id, bool isActive)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó cambiar el estado de un historial de pago con ID inválido.");
                throw new ValidationException("id", "El ID debe ser mayor que cero.");
            }

            try
            {
                var Bill = await _billData.GetByIdAsync(id);
                if (Bill == null)
                {
                    throw new EntityNotFoundException("module", id);
                }

                // Actualizar el estado activo
                Bill.IsActive = isActive;

                var isUpdated = await _billData.UpdateAsync(Bill);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo cambiar el estado del historial de pago con ID {id}.");
                }

                return MapToDTO(Bill);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar el estado del historial de pago con ID: {Bill}", id);
                throw new ExternalServiceException("Base de datos", $"Error al cambiar el estado del historial de pago con ID {id}.", ex);
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
                    IsActive = billDto.IsActive

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
                State = bill.State,
                IsActive = bill.IsActive
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
