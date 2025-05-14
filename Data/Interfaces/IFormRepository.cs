using Data.Core;
using Entity.Dto;
using Entity.Model;


namespace Data.Interfaces
{
    public interface IFormRepository : IServiceBase<Form>
    {
        Task<FormDto> Update(int id, string name, string? description, string? status);
    }
}

