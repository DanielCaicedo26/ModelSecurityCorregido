using Bussines.Core;
using Bussines.interfaces;
using Data.Interfaces;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;


namespace Bussines.Services
{
    public class UserBusiness : ServiceBase<UserDto, User>, IUserBusiness
    {
        public UserBusiness(IUserRepository repository, ILogger<UserBusiness> logger)
            : base(repository, logger) { }
    }
}


