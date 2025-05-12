using Business.Core;
using Entity.Dto;
using Entity.Model;

namespace Business.Interfaces
{
    public interface IAccessLogBusiness : IServiceBase<AccessLogDto, AccessLog>
    {
        Task<IEnumerable<AccessLogDto>> GetAllAccessLogsAsync();
        Task<AccessLogDto> GetAccessLogByIdAsync(int id);
        Task<AccessLogDto> CreateAccessLogAsync(AccessLogDto accessLogDto, int userId);
        Task<bool> UpdateAccessLogAsync(AccessLogDto accessLogDto);
        Task<bool> DeleteAccessLogAsync(int id);
        Task<AccessLogDto> Update(int id, string action, bool status, string details);
    }
}