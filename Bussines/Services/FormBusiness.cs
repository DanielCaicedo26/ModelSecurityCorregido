using Bussines.Core;
using Bussines.interfaces;
using Data.Interfaces;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;

namespace Bussines.Services
{
    public class FormBusiness : ServiceBase<FormDto, Form>, IFormBusiness
    {
        public FormBusiness(IFormRepository repository, ILogger<FormBusiness> logger)
            : base(repository, logger)
        {
        }
    }
    
    }

