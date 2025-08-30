using Entity.Context;
using Entity.Dto;
using Entity.Model;
using Microsoft.EntityFrameworkCore;

namespace Web2.Services
{
    public interface IDynamicPersonService
    {
        Task<IEnumerable<PersonDto>> GetAllAsync();
        Task<PersonDto?> GetByIdAsync(int id);
    }

    public class DynamicPersonService : IDynamicPersonService
    {
        private readonly IDatabaseSelectorService _databaseSelector;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DynamicPersonService> _logger;

        public DynamicPersonService(
            IDatabaseSelectorService databaseSelector,
            IServiceProvider serviceProvider,
            ILogger<DynamicPersonService> logger)
        {
            _databaseSelector = databaseSelector;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task<IEnumerable<PersonDto>> GetAllAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = _databaseSelector.GetCurrentContext(scope.ServiceProvider);
            
            _logger.LogInformation($"Obteniendo personas desde {_databaseSelector.CurrentEngine}");
            
            var persons = await context.Set<Person>()
                .Where(p => p.IsActive)
                .Select(p => new PersonDto
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    DocumentNumber = p.DocumentNumber,
                    DocumentType = p.DocumentType,
                    Phone = p.Phone,
                    IsActive = p.IsActive
                })
                .ToListAsync();

            return persons;
        }

        public async Task<PersonDto?> GetByIdAsync(int id)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = _databaseSelector.GetCurrentContext(scope.ServiceProvider);
            
            _logger.LogInformation($"Obteniendo persona ID {id} desde {_databaseSelector.CurrentEngine}");
            
            var person = await context.Set<Person>()
                .Where(p => p.Id == id && p.IsActive)
                .Select(p => new PersonDto
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    DocumentNumber = p.DocumentNumber,
                    DocumentType = p.DocumentType,
                    Phone = p.Phone,
                    IsActive = p.IsActive
                })
                .FirstOrDefaultAsync();

            return person;
        }
    }
}