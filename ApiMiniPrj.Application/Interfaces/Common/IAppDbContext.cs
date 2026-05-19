namespace ApiMiniPrj.Application.Interfaces.Common
{
    public interface IAppDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
