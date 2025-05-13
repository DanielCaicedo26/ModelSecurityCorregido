using Bussines.Core;
using Bussines.interfaces;
using Data.Interfaces;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;

namespace Bussines.Services
{
    /// <summary>
    /// Implementación del servicio de negocio para los registros de acceso utilizando únicamente métodos genéricos.
    /// </summary>
    public class PersonBusiness : ServiceBase<PersonDto, Person>, IPersonBusiness
    {
        public PersonBusiness(IPersonRepository repository, ILogger<PersonBusiness> logger)
            : base(repository, logger) { }
    }
}
