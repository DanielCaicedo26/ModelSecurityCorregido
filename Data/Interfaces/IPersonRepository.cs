using Data.Core;
using Entity.Model;

namespace Data.Interfaces
{
    public interface IPersonRepository : IServiceBase<Person>
    {
        Task<IEnumerable<Person>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Person>> GetByDocumentNumberAsync(string documentNumber); // Add this method
    }
}

    

    

