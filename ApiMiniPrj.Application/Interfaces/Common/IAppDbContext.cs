
namespace ApiMiniPrj.Application.Interfaces.Common
{
    public interface IAppDbContext
    {
        DbSet<Event> Events { get; }
        DbSet<Organizer> Organizers { get; }
        DbSet<Ticket> Tickets { get; }
        DbSet<RefreshToken> RefreshTokens { get; }
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
