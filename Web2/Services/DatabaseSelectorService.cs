using Entity.Context;
using Microsoft.EntityFrameworkCore;

namespace Web2.Services
{
    public interface IDatabaseSelectorService
    {
        string CurrentEngine { get; }
        void SetEngine(string engine);
        DbContext GetCurrentContext(IServiceProvider serviceProvider);
        IEnumerable<string> GetAvailableEngines();
    }

    public class DatabaseSelectorService : IDatabaseSelectorService
    {
        private string _currentEngine;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DatabaseSelectorService> _logger;

        public DatabaseSelectorService(IConfiguration configuration, ILogger<DatabaseSelectorService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _currentEngine = _configuration["DB_ENGINE"] ?? "sqlserver";
        }

        public string CurrentEngine => _currentEngine;

        public void SetEngine(string engine)
        {
            var normalizedEngine = engine.ToLower();
            if (!GetAvailableEngines().Contains(normalizedEngine))
            {
                throw new ArgumentException($"Motor de base de datos no soportado: {engine}");
            }

            _currentEngine = normalizedEngine;
            _logger.LogInformation($"Motor de base de datos cambiado a: {_currentEngine}");
        }

        public DbContext GetCurrentContext(IServiceProvider serviceProvider)
        {
            return _currentEngine.ToLower() switch
            {
                "postgres" => serviceProvider.GetRequiredService<ApplicationDbContextPostgres>(),
                "mysql" => serviceProvider.GetRequiredService<ApplicationDbContextMySql>(),
                "sqlserver" => serviceProvider.GetRequiredService<ApplicationDbContext>(),
                _ => serviceProvider.GetRequiredService<ApplicationDbContext>()
            };
        }

        public IEnumerable<string> GetAvailableEngines()
        {
            return new[] { "sqlserver", "postgres", "mysql" };
        }
    }
}