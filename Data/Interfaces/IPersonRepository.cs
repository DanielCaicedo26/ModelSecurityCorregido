using Data.Core;
using Entity.Model;

namespace Data.Interfaces
{
    public interface IPersonRepository : IRepository<Person>
    {
        Task<IEnumerable<Person>> GetByUserIdAsync(int userId);
    }
}
    

    

