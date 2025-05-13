using Bussines.Core;
using Entity.Dto;
using Entity.Model;

namespace Bussines.interfaces
{
    public interface IAccessLogBusiness : IServiceBase<AccessLogDto, AccessLog>
    {
        // Sin métodos adicionales, solo los definidos en IServiceBase.
    }
}
