using Bussines.Core;
using Entity.Dto;
using Entity.Model;

namespace Bussines.interfaces
{
    public interface IFormBusiness : IServiceBase<FormDto, Form>
    {
        Task<FormDto> Update(int id, string name, string? description, string? status);
    }
}
