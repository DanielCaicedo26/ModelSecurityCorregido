using Data.Core;
using Entity.Model;

namespace Data.Interfaces
{
    public interface IAccessLogRepository : IRepository<AccessLog>
    {
        Task<IEnumerable<AccessLog>> GetByUserIdAsync(int userId);
    }
}