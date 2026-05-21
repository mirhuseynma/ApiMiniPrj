namespace ApiMiniPrj.Application.DTOs.Auth
{
    public class RegisterDto
    {
        public string FullName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string RePassword { get; set; } = null!;
        public bool AcceptTerms { get; set; }
    }
}
