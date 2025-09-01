using Entity.Context;
using Microsoft.EntityFrameworkCore;

namespace Entity.Services
{
    public interface IDynamicDbContextService
    {
        DbContext GetCurrentContext();
        ApplicationDbContext GetCurrentApplicationContext();
    }
}