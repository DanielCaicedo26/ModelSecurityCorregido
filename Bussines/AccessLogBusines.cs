using Data;
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
        /// <param name="accessLogData">Instancia de AccessLogData para acceder a los datos de los registros de acceso.</param>
        /// <param name="logger">Instancia de ILogger para el registro de logs.</param>
        public AccessLogBusiness(AccessLogData accessLogData, ILogger<AccessLogBusiness> logger)
        {
            _accessLogData = accessLogData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros de acceso de manera asíncrona.
        /// </summary>
        /// <returns>Una lista de objetos AccessLogDto.</returns>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar los registros de acceso.</exception>
        public async Task<IEnumerable<AccessLogDto>> GetAllAccessLogsAsync()
        {
            try
            {
                var accessLogs = await _accessLogData.GetAllAsync();
                return MapToDTOList(accessLogs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los registros de acceso");
                throw new ExternalServiceException("Base de datos", "Error al recuperar los registros de acceso", ex);
            }
        }

        /// <summary>
        /// Obtiene un registro de acceso por su ID de manera asíncrona.
        /// </summary>
        /// <param name="id">El ID del registro de acceso.</param>
        /// <returns>Un objeto AccessLogDto.</returns>
        /// <exception cref="ValidationException">Lanzada cuando el ID es inválido.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada cuando no se encuentra el registro de acceso.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al recuperar el registro de acceso.</exception>
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
        /// <param name="accessLogDto">El objeto AccessLogDto con los datos del registro de acceso.</param>
        /// <param name="userId">El ID del usuario que realiza la acción.</param>
        /// <param name="details">Detalles adicionales de la acción.</param>
        /// <returns>El objeto AccessLogDto creado.</returns>
        /// <exception cref="ValidationException">Lanzada cuando los datos del registro de acceso son inválidos.</exception>
        /// <exception cref="ExternalServiceException">Lanzada cuando ocurre un error al crear el registro de acceso.</exception>
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
                    Details = details
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
        /// <param name="accessLogDto">El objeto AccessLogDto con los datos del registro de acceso.</param>
        /// <param name="userId">El ID del usuario que realiza la acción.</param>
        /// <exception cref="ValidationException">Lanzada cuando los datos del registro de acceso son inválidos.</exception>
        private void ValidateAccessLog(AccessLogDto accessLogDto, int userId)
        {
            if (accessLogDto == null)
            {
                throw new ValidationException("El objeto AccessLog no puede ser nulo");
            }

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
        /// <param name="accessLog">El objeto AccessLog a mapear.</param>
        /// <returns>El objeto AccessLogDto mapeado.</returns>
        private static AccessLogDto MapToDTO(AccessLog accessLog)
        {
            return new AccessLogDto
            {
                Id = accessLog.Id,
                UserId = accessLog.UserId,
                Action = accessLog.Action,
                Status = accessLog.Status
            };
        }

        /// <summary>
        /// Mapea una lista de objetos AccessLog a una lista de AccessLogDto.
        /// </summary>
        /// <param name="accessLogs">La lista de objetos AccessLog a mapear.</param>
        /// <returns>La lista de objetos AccessLogDto mapeados.</returns>
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
