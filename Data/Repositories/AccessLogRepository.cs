using Data.Core;
using Data.Interfaces;
using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repositories
{
    public class AccessLogRepository : GenericRepository<AccessLog>, IAccessLogRepository
    {
        public AccessLogRepository(ApplicationDbContext context, ILogger<AccessLogRepository> logger)
            : base(context, logger)
        {
        }

        public override async Task<IEnumerable<AccessLog>> GetAllAsync()
        {
            return await _context.AccessLog
                .Include(a => a.User)
                .AsNoTracking()
                .ToListAsync();
        }

        public override async Task<AccessLog?> GetByIdAsync(int id)
        {
            return await _context.AccessLog
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<AccessLog>> GetByUserIdAsync(int userId)
        {
            return await _context.AccessLog
                .Where(a => a.UserId == userId)
                .Include(a => a.User)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}