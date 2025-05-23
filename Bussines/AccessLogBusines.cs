﻿using Data;
using Entity.Dto;
using Microsoft.Extensions.Logging;
using Entity.Model;
using Utilities.Exceptions;

namespace Bussines
{
    /// <summary>
    /// Clase que maneja la lógica de negocio para los registros de acceso.
    /// </summary>
    public class AccessLogBusiness
    {
        private readonly AccessLogData _accessLogData;
        private readonly ILogger<AccessLogBusiness> _logger;

        /// <summary>
        /// Constructor de la clase AccessLogBusiness.
        /// </summary>
        public AccessLogBusiness(AccessLogData accessLogData, ILogger<AccessLogBusiness> logger)
        {
            _accessLogData = accessLogData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros de acceso de manera asíncrona.
        /// </summary>
        public async Task<IEnumerable<AccessLogDto>> GetAllAccessLogsAsync()
        {
            try
            {
                var accessLogs = await _accessLogData.GetAllAsync();
                var visibleaccesLogs = accessLogs.Where(m => m.IsActive);
                return MapToDTOList(accessLogs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los registros de acceso");
                throw new ExternalServiceException("Base de datos", "Error al recuperar los registros de acceso", ex);
            }
        }

        /// <summary>
        /// Actualiza un registro de acceso existente de manera asíncrona.
        /// </summary>
        public async Task UpdateAccessLogAsync(AccessLogDto accessLogDto)
        {
            if (accessLogDto == null || accessLogDto.Id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar un AccessLog con datos inválidos");
                throw new ValidationException("AccessLogDto", "El objeto AccessLogDto no es válido");
            }

            try
            {
                var existingLog = await _accessLogData.GetByIdAsync(accessLogDto.Id);
                if (existingLog == null)
                {
                    _logger.LogInformation("No se encontró ningún AccessLog con ID: {AccessLogId}", accessLogDto.Id);
                    throw new EntityNotFoundException("AccessLog", accessLogDto.Id);
                }

                existingLog.Action = accessLogDto.Action;
                existingLog.Status = accessLogDto.Status;

                var success = await _accessLogData.UpdateAsync(existingLog);
                if (!success)
                {
                    throw new ExternalServiceException("Base de datos", "Error al actualizar el AccessLog");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el AccessLog con ID: {AccessLogId}", accessLogDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el AccessLog con ID {accessLogDto.Id}", ex);
            }
        }

        /// <summary>
        /// Elimina un registro de acceso por su ID de manera asíncrona.
        /// </summary>
        public async Task DeleteAccessLogAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un AccessLog con ID inválido: {AccessLogId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var existingLog = await _accessLogData.GetByIdAsync(id);
                if (existingLog == null)
                {
                    _logger.LogInformation("No se encontró ningún AccessLog con ID: {AccessLogId}", id);
                    throw new EntityNotFoundException("AccessLog", id);
                }

                var success = await _accessLogData.DeleteAsync(id);
                if (!success)
                {
                    throw new ExternalServiceException("Base de datos", "Error al eliminar el AccessLog");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el AccessLog con ID: {AccessLogId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el AccessLog con ID {id}", ex);
            }
        }

        public async Task<AccessLogDto> Update(int id, string action, bool status, string? details)
        {
            if (id <= 0 || string.IsNullOrWhiteSpace(action))
            {
                throw new ValidationException("Datos inválidos", "El ID debe ser mayor que cero y la acción no puede estar vacía.");
            }

            try
            {
                var accessLog = await _accessLogData.GetByIdAsync(id);
                if (accessLog == null)
                {
                    throw new EntityNotFoundException("AccessLog", id);
                }

                accessLog.Action = action;
                accessLog.Status = status;
                accessLog.Details = details;
                accessLog.CreatedAt = DateTime.UtcNow; // O la fecha que quieras asignar

                var isUpdated = await _accessLogData.UpdateAsync(accessLog);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo actualizar el AccessLog con ID {id}");
                }

                return MapToDTO(accessLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el AccessLog con ID: {AccessLogId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el AccessLog con ID {id}", ex);
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
        public async Task<AccessLogDto> SetActiveStatusAsync(int id, bool isActive)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó cambiar el estado de un historial de pago con ID inválido.");
                throw new ValidationException("id", "El ID debe ser mayor que cero.");
            }

            try
            {
                var accessLog = await _accessLogData.GetByIdAsync(id);
                if (accessLog == null)
                {
                    throw new EntityNotFoundException("module", id);
                }

                // Actualizar el estado activo
                accessLog.IsActive = isActive;

                var isUpdated = await _accessLogData.UpdateAsync(accessLog);
                if (!isUpdated)
                {
                    throw new ExternalServiceException("Base de datos", $"No se pudo cambiar el estado del historial de pago con ID {id}.");
                }

                return MapToDTO(accessLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar el estado del historial de pago con ID: {Bill}", id);
                throw new ExternalServiceException("Base de datos", $"Error al cambiar el estado del historial de pago con ID {id}.", ex);
            }
        }




        /// <summary>
        /// Obtiene un registro de acceso por su ID de manera asíncrona.
        /// </summary>
        public async Task<AccessLogDto> GetAccessLogByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un AccessLog con ID inválido: {AccessLogId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var accessLog = await _accessLogData.GetByIdAsync(id);
                if (accessLog == null)
                {
                    _logger.LogInformation("No se encontró ningún AccessLog con ID: {AccessLogId}", id);
                    throw new EntityNotFoundException("AccessLog", id);
                }

                return MapToDTO(accessLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el AccessLog con ID: {AccessLogId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el AccessLog con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo registro de acceso de manera asíncrona.
        /// </summary>
        public async Task<AccessLogDto> CreateAccessLogAsync(AccessLogDto accessLogDto, int userId, string? details = null)
        {
            try
            {
                ValidateAccessLog(accessLogDto, userId);

                var accessLog = new AccessLog
                {
                    UserId = userId,
                    Action = accessLogDto.Action,
                    Status = accessLogDto.Status,
                    Timestamp = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    Details = accessLogDto.Details,
                    IsActive = accessLogDto.IsActive
                    
                };

                var createdAccessLog = await _accessLogData.CreateAsync(accessLog);
                return MapToDTO(createdAccessLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo AccessLog");
                throw new ExternalServiceException("Base de datos", "Error al crear el AccessLog", ex);
            }
        }

        /// <summary>
        /// Valida los datos del registro de acceso.
        /// </summary>
        private void ValidateAccessLog(AccessLogDto accessLogDto, int userId)
        {
            if (accessLogDto == null)
                throw new ValidationException("El objeto AccessLog no puede ser nulo");

            if (string.IsNullOrWhiteSpace(accessLogDto.Action))
            {
                _logger.LogWarning("Se intentó crear un AccessLog con acción vacía");
                throw new ValidationException("Action", "La acción no puede estar vacía");
            }

            if (userId <= 0)
            {
                _logger.LogWarning("Se intentó crear un AccessLog con UserId inválido");
                throw new ValidationException("UserId", "El UserId debe ser mayor que cero");
            }
        }

        /// <summary>
        /// Mapea un objeto AccessLog a AccessLogDto.
        /// </summary>
        private static AccessLogDto MapToDTO(AccessLog accessLog)
        {
            return new AccessLogDto
            {
                Id = accessLog.Id,
                UserId = accessLog.UserId,
                Action = accessLog.Action,
                Status = accessLog.Status,
                Details = accessLog.Details,
                IsActive = accessLog. IsActive
            };
        }

        /// <summary>
        /// Mapea una lista de objetos AccessLog a una lista de AccessLogDto.
        /// </summary>
        private IEnumerable<AccessLogDto> MapToDTOList(IEnumerable<AccessLog> accessLogs)
        {
            var accessLogsDto = new List<AccessLogDto>();
            foreach (var log in accessLogs)
            {
                accessLogsDto.Add(MapToDTO(log));
            }
            return accessLogsDto;
        }
    }
}
