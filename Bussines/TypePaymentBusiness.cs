using Data;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Bussines
{
    /// <summary>
    /// Clase que maneja la lógica de negocio para los tipos de pago.
    /// </summary>
    public class TypePaymentBusiness
    {
        private readonly TypePaymentData _typePaymentData;
        private readonly ILogger<TypePaymentBusiness> _logger;

        /// <summary>
        /// Constructor de la clase TypePaymentBusiness.
        /// </summary>
        /// <param name="typePaymentData">Instancia de TypePaymentData para acceder a los datos de los tipos de pago.</param>
        /// <param name="logger">Instancia de ILogger para el registro de logs.</param>
        public TypePaymentBusiness(TypePaymentData typePaymentData, ILogger<TypePaymentBusiness> logger)
        {
            _typePaymentData = typePaymentData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los tipos de pago de manera asíncrona.
        /// </summary>
        /// <returns>Una lista de objetos TypePaymentDto.</returns>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar los tipos de pago.</exception>
        public async Task<IEnumerable<TypePaymentDto>> GetAllTypePaymentsAsync()
        {
            try
            {
                var typePayments = await _typePaymentData.GetAllAsync();
                var visibletypePayments = typePayments.Where(n => n.IsActive);
                return MapToDTOList(visibletypePayments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los tipos de pago");
                throw new ExternalServiceException("Base de datos", "Error al recuperar los tipos de pago", ex);
            }
        }


        /// <summary>
        /// Obtiene un tipo de pago por su ID de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID del tipo de pago.</param>
        /// <returns>Un objeto TypePaymentDto.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el tipo de pago.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar el tipo de pago.</exception>
        public async Task<TypePaymentDto> GetTypePaymentByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un tipo de pago con ID inválido: {TypePaymentId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var typePayment = await _typePaymentData.GetByIdAsync(id);
                if (typePayment == null)
                {
                    _logger.LogInformation("No se encontró ningún tipo de pago con ID: {TypePaymentId}", id);
                    throw new EntityNotFoundException("TypePayment", id);
                }

                return MapToDTO(typePayment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el tipo de pago con ID: {TypePaymentId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el tipo de pago con ID {id}", ex);
            }
        }

        /// <summary>
        /// Elimina un tipo de pago por su ID.
        /// </summary>
        /// <param name="id">El ID del tipo de pago a eliminar.</param>
        /// <exception cref="ValidationException">Lanzada si el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra el tipo de pago.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al eliminar el tipo de pago.</exception>
        public async Task DeleteTypePaymentAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un tipo de pago con ID inválido: {TypePaymentId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero.");
            }

            try
            {
                // Verificar si el tipo de pago existe
                var typePayment = await _typePaymentData.GetByIdAsync(id);
                if (typePayment == null)
                {
                    _logger.LogInformation("No se encontró ningún tipo de pago con ID: {TypePaymentId}", id);
                    throw new EntityNotFoundException("TypePayment", id);
                }

                // Intentar eliminar el tipo de pago
                var isDeleted = await _typePaymentData.DeleteAsync(id);
                if (!isDeleted)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo eliminar el tipo de pago con ID {id}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el tipo de pago con ID: {TypePaymentId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el tipo de pago con ID {id}.", ex);
            }
        }

        /// <summary>
        /// Activa o desactiva un tipo de pago por su ID.
        /// </summary>
        /// <param name="id">El ID del tipo de pago.</param>
        /// <param name="isActive">Estado deseado: true para activar, false para desactivar.</param>
        /// <returns>El objeto TypePaymentDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra el tipo de pago.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar el estado.</exception>
        public async Task<TypePaymentDto> SetActiveStatusAsync(int id, bool isActive)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó cambiar el estado de un tipo de pago con ID inválido: {TypePaymentId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var typePayment = await _typePaymentData.GetByIdAsync(id);
                if (typePayment == null)
                {
                    throw new EntityNotFoundException("TypePayment", id);
                }

                // Actualizar el estado activo
                typePayment.IsActive = isActive;

                var isUpdated = await _typePaymentData.UpdateAsync(typePayment);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo cambiar el estado del tipo de pago con ID {id}");
                }

                return MapToDTO(typePayment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar el estado del tipo de pago con ID: {TypePaymentId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al cambiar el estado del tipo de pago con ID {id}", ex);
            }
        }


        /// <summary>
        /// Actualiza los datos de un tipo de pago.
        /// </summary>
        /// <param name="typePaymentDto">El objeto TypePaymentDto con los datos actualizados del tipo de pago.</param>
        /// <returns>El objeto TypePaymentDto actualizado.</returns>
        /// <exception cref="ValidationException">Lanzada si los datos son inválidos.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si no se encuentra el tipo de pago.</exception>
        /// <exception cref="ExternalServiceException">Lanzada si ocurre un error al actualizar el tipo de pago.</exception>
        public async Task<TypePaymentDto> UpdateTypePaymentAsync(TypePaymentDto typePaymentDto)
        {
            if (typePaymentDto == null || typePaymentDto.Id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar un tipo de pago con datos inválidos o ID inválido.");
                throw new ValidationException("id", "El ID debe ser mayor que cero y los datos no pueden ser nulos.");
            }

            try
            {
                // Verificar si el tipo de pago existe
                var existingTypePayment = await _typePaymentData.GetByIdAsync(typePaymentDto.Id);
                if (existingTypePayment == null)
                {
                    _logger.LogInformation("No se encontró ningún tipo de pago con ID: {TypePaymentId}", typePaymentDto.Id);
                    throw new EntityNotFoundException("TypePayment", typePaymentDto.Id);
                }

                // Actualizar los datos del tipo de pago
                existingTypePayment.Name = typePaymentDto.Name;
                existingTypePayment.Description = typePaymentDto.Description;

                var isUpdated = await _typePaymentData.UpdateAsync(existingTypePayment);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar el tipo de pago con ID {typePaymentDto.Id}.");
                }

                return MapToDTO(existingTypePayment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el tipo de pago con ID: {TypePaymentId}", typePaymentDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el tipo de pago con ID {typePaymentDto.Id}.", ex);
            }
        }

        public async Task<TypePaymentDto> Update(int id, string Name, string Description)
        {
            if (id <= 0 || string.IsNullOrWhiteSpace(Name))
            {
                throw new ValidationException("Datos inválidos", "ID debe ser mayor que cero y el mensaje no puede estar vacío");
            }

            try
            {
                var typePayment = await _typePaymentData.GetByIdAsync(id);
                if ( typePayment == null)
                {
                    throw new EntityNotFoundException("typePayment", id);
                }

                typePayment.Name = Name;
                typePayment.Description = Description;

                var isUpdated = await _typePaymentData.UpdateAsync(typePayment);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar la notificación con ID {id}");
                }

                return MapToDTO(typePayment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la notificación con ID: {UserNotificationId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la notificación con ID {id}", ex);
            }
        }




        /// <summary>
        /// Crea un nuevo tipo de pago de manera asíncrona.
        /// </summary>
        /// <param name="typePaymentDto">El objeto TypePaymentDto con los datos del tipo de pago.</param>
        /// <returns>El objeto TypePaymentDto creado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando los datos del tipo de pago son inválidos.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al crear el tipo de pago.</exception>
        public async Task<TypePaymentDto> CreateTypePaymentAsync(TypePaymentDto typePaymentDto)
        {
            try
            {
                ValidateTypePayment(typePaymentDto);

                var typePayment = new TypePayment
                {
                    Name = typePaymentDto.Name,
                    Description = typePaymentDto.Description,
                    IsActive = typePaymentDto.IsActive
                };

                var createdTypePayment = await _typePaymentData.CreateAsync(typePayment);
                return MapToDTO(createdTypePayment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo tipo de pago");
                throw new ExternalServiceException("Base de datos", "Error al crear el tipo de pago", ex);
            }
        }

        /// <summary>
        /// Valida los datos del tipo de pago.
        /// </summary>
        /// <param name="typePaymentDto">El objeto TypePaymentDto con los datos del tipo de pago.</param>
        /// <exception cref="ValidationException">Lanzada cuando los datos del tipo de pago son inválidos.</exception>
        private void ValidateTypePayment(TypePaymentDto typePaymentDto)
        {
            if (typePaymentDto == null)
            {
                throw new ValidationException("El objeto TypePayment no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(typePaymentDto.Name))
            {
                _logger.LogWarning("Se intentó crear un tipo de pago con nombre vacío");
                throw new ValidationException("Name", "El nombre no puede estar vacío");
            }
        }

        /// <summary>
        /// Mapea un objeto TypePayment a TypePaymentDto.
        /// </summary>
        /// <param name="typePayment">El objeto TypePayment a mapear.</param>
        /// <returns>El objeto TypePaymentDto mapeado.</returns>
        private static TypePaymentDto MapToDTO(TypePayment typePayment)
        {
            return new TypePaymentDto
            {
                Id = typePayment.Id,
                Name = typePayment.Name,
                Description = typePayment.Description,
                IsActive = typePayment.IsActive
            };
        }

        /// <summary>
        /// Mapea una lista de objetos TypePayment a una lista de TypePaymentDto.
        /// </summary>
        /// <param name="typePayments">La lista de objetos TypePayment a mapear.</param>
        /// <returns>La lista de objetos TypePaymentDto mapeados.</returns>
        private static IEnumerable<TypePaymentDto> MapToDTOList(IEnumerable<TypePayment> typePayments)
        {
            var typePaymentsDto = new List<TypePaymentDto>();
            foreach (var typePayment in typePayments)
            {
                typePaymentsDto.Add(MapToDTO(typePayment));
            }
            return typePaymentsDto;
        }
    }
}









