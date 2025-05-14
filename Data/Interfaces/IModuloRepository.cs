using Data.Core;
using Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IModuloRepository : IRepository<Module>

    {
        Task<IEnumerable<Module>> GetByUserIdAsync(int userId);
    }
}
