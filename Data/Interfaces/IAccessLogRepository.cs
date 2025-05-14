using Data.Core;
using Entity.Model;

namespace Data.Interfaces
{
    public interface IAccessLogRepository : IServiceBase<AccessLog>
    {
        Task<IEnumerable<AccessLog>> GetByUserIdAsync(int userId);
    }
}