using Bussines.Core;
using Data.Core;
using Entity.Dto;
using Entity.Model;

namespace Bussines.interfaces
{
    public interface IModuleBusiness : IServiceBase<ModuleDto, Module>
    {
        Task<IEnumerable<Module>> GetByUserIdAsync(int userId);
        Task<ModuleDto> Update(int id, string name, string? description, string? status);
    }
}