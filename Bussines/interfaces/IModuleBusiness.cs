using Data.Core;
using Entity.Model;

namespace Bussines.interfaces
{
    public interface IModuleBusiness : IServiceBase<Module>
    {
        Task<IEnumerable<Module>> GetByUserIdAsync(int userId);
    }
}