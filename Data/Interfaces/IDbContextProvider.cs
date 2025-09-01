using Entity.Context;

namespace Data.Interfaces
{
    public interface IDbContextProvider
    {
        ApplicationDbContext GetDbContext();
    }
}