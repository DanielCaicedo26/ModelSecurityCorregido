using Business.Core;
using Bussines.Core;
using Bussines.interfaces;
using Data.Interfaces;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;

namespace Bussines.Services
{
    /// <summary>
    /// Implementación del servicio de negocio para los registros de acceso utilizando únicamente métodos genéricos.
    /// </summary>
    public class AccessLogBusiness : ServiceBase<AccessLogDto, AccessLog>, IAccessLogBusiness
    {
        public AccessLogBusiness(IAccessLogRepository repository, ILogger<AccessLogBusiness> logger)
            : base(repository, logger) { }
    }
}
