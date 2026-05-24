
namespace ApiMiniPrj.Domain.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = null!;
        public DateTime Expires { get; set; }
        public string UserId { get; set; } = null!;
        public AppUser? User { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime CreatedAt { get; set; }

    }
}
