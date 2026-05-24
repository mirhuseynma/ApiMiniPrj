using Microsoft.EntityFrameworkCore;

namespace ApiMiniPrj.Application.Interfaces.Common
{
    public interface IAppDbContext
    {
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
