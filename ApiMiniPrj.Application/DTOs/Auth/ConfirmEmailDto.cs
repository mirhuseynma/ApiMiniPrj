namespace ApiMiniPrj.Application.DTOs.Auth
{
    public class ConfirmEmailDto
    {
        public string Token { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
