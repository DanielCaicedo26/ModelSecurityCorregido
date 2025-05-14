using Bussines.Core;
using Bussines.interfaces;
using Data.Interfaces;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;

namespace Bussines.Services
{
    public class RoleUserBusiness : ServiceBase<RoleUserDto, RoleUser>, IRoleUserBusiness
    {
        public RoleUserBusiness(IRoleUserRepository repository, ILogger<RoleUserBusiness> logger)
            : base(repository, logger)
        {
        }
    }
}