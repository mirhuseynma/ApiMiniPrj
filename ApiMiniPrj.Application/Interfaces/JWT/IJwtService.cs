
namespace ApiMiniPrj.Application.Interfaces.JWT
{
    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(AppUser user);
    }
}
