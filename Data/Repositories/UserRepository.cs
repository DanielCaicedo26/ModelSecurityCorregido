
    using Data.Core;
    using Data.Interfaces;
    using Entity.Context;
    using Entity.Model;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    namespace Data.Repositories
    {
        public class UserRepository : GenericRepository<User>, IUserRepository
        {
            public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
                : base(context, logger)
            {
            }
        // esta en prueba 
        public async Task<IEnumerable<User>> GetByUserIdAsync(int userId)
        {
            return await _context.User
                .Where(u => u.Id == userId)
                .Include(u => u.Person)
                .Include(u => u.RoleUsers)
                .Include(u => u.UserNotifications)
                .Include(u => u.PaymentHistories)
                .AsNoTracking()
                .ToListAsync();
        }


            public override async Task<IEnumerable<User>> GetAllAsync()
            {
                var users = await _context.User
                    .Include(u => u.Person)
                    .Include(u => u.RoleUsers)
                    .Include(u => u.UserNotifications)
                    .Include(u => u.PaymentHistories)
                    .AsNoTracking()
                    .ToListAsync();

                foreach (var user in users)
                {
                    user.Username ??= $"usuario-{user.Id}";
                    user.Email ??= $"email-{user.Id}@ejemplo.com";
                    user.Password ??= "password-placeholder";
                }

                return users;
            }

            public override async Task<User?> GetByIdAsync(int id)
            {
                var user = await _context.User
                    .Include(u => u.Person)
                    .Include(u => u.RoleUsers)
                    .Include(u => u.UserNotifications)
                    .Include(u => u.PaymentHistories)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user != null)
                {
                    user.Username ??= $"usuario-{user.Id}";
                    user.Email ??= $"email-{user.Id}@ejemplo.com";
                    user.Password ??= "password-placeholder";
                }

                return user;
            }

            public async Task<User?> GetByUsernameAsync(string username)
            {
                return await _context.User
                    .Include(u => u.Person)
                    .FirstOrDefaultAsync(u => u.Username == username);
            }

            public async Task<User> CreateAsync(User user)
            {
                user.Username ??= $"usuario-{DateTime.Now.Ticks}";
                user.Email ??= $"email-{DateTime.Now.Ticks}@ejemplo.com";
                user.Password ??= "password-placeholder";

                await _context.User.AddAsync(user);
                await _context.SaveChangesAsync();
                return user;
            }

            public override async Task<bool> UpdateAsync(User user)
            {
                var existingUser = await _context.User.FindAsync(user.Id);
                if (existingUser == null)
                    return false;

                user.Username ??= $"usuario-{user.Id}";
                user.Email ??= $"email-{user.Id}@ejemplo.com";
                user.Password ??= existingUser.Password;

                _context.Entry(existingUser).CurrentValues.SetValues(user);
                await _context.SaveChangesAsync();
                return true;
            }

            public override async Task<bool> DeleteAsync(int id)
            {
                var user = await _context.User.FindAsync(id);
                if (user == null)
                    return false;

                _context.User.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
        }
    }



