using Entity.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Data.Interfaces;

namespace Data.Core
{
    public class GenericRepository<T> : IServiceBase<T> where T : class
    {
        protected readonly IDbContextProvider _dbContextProvider;
        protected readonly ILogger _logger;

        public GenericRepository(IDbContextProvider dbContextProvider, ILogger<GenericRepository<T>> logger)
        {
            _dbContextProvider = dbContextProvider;
            _logger = logger;
        }

        protected ApplicationDbContext _context => _dbContextProvider.GetDbContext();

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<bool> UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null) return false;

            _context.Set<T>().Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> DeleteLogicalAsync(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null) return false;

            var propIsActive = entity.GetType().GetProperty("IsActive");
            if (propIsActive != null)
            {
                propIsActive.SetValue(entity, false);
                await _context.SaveChangesAsync();
                return true;
            }

            _logger.LogWarning($"La entidad {typeof(T).Name} no tiene propiedad IsActive");
            return false;
        }
    }
}