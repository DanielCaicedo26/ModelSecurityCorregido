using Data.Core;
using Entity.Dto;
using Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IModuleRepository : IServiceBase<Module>

    {
        Task<IEnumerable<Module>> GetByUserIdAsync(int userId);
        
    }
}
