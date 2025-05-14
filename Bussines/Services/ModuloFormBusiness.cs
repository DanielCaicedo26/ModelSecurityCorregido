using Bussines.Core;
using Bussines.interfaces;
using Data.Interfaces;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;

namespace Bussines.Services
{
    public class ModuloFormBusiness : ServiceBase<ModuloFormDto, ModuloForm>, IModuloFormBusiness
    {
        public ModuloFormBusiness(IModuloFormRepository repository, ILogger<ModuloFormBusiness> logger)
            : base(repository, logger)
        {
        }
    }
}