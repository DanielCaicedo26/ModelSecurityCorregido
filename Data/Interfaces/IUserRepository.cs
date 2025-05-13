using Data.Core;
using Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IUserRepository: IRepository <User>
    {
        Task<IEnumerable<User>> GetByUserIdAsync(int userId);
        
    }
}
