using Entity.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Data.Interfaces;

namespace Web2.Services
{
    public class PerRequestDbContextProvider : IDbContextProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider _serviceProvider;

        public PerRequestDbContextProvider(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _serviceProvider = serviceProvider;
        }

        public ApplicationDbContext GetDbContext()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                // Fallback for scenarios without an HTTP context (like background services or startup)
                // Defaulting to SQL Server as a safe bet.
                return _serviceProvider.GetRequiredService<ApplicationDbContext>();
            }

            var dbEngineHeader = httpContext.Request.Headers["X-Database-Engine"].FirstOrDefault();

            return dbEngineHeader?.ToLower() switch
            {
                "postgres" => _serviceProvider.GetRequiredService<ApplicationDbContextPostgres>(),
                "mysql" => _serviceProvider.GetRequiredService<ApplicationDbContextMySql>(),
                _ => _serviceProvider.GetRequiredService<ApplicationDbContext>() // Default to SQL Server
            };
        }
    }
}
