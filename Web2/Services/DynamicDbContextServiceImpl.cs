using Entity.Context;
using Entity.Services;
using Microsoft.EntityFrameworkCore;

namespace Web2.Services
{
    public class DynamicDbContextServiceImpl : IDynamicDbContextService
    {
        private readonly IDatabaseSelectorService _databaseSelector;
        private readonly IServiceProvider _serviceProvider;

        public DynamicDbContextServiceImpl(IDatabaseSelectorService databaseSelector, IServiceProvider serviceProvider)
        {
            _databaseSelector = databaseSelector;
            _serviceProvider = serviceProvider;
        }

        public DbContext GetCurrentContext()
        {
            return _databaseSelector.GetCurrentContext(_serviceProvider);
        }

        public ApplicationDbContext GetCurrentApplicationContext()
        {
            return (ApplicationDbContext)GetCurrentContext();
        }
    }
}