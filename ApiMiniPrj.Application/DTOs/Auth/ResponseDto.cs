namespace ApiMiniPrj.Application.DTOs.Auth
{
    public class ResponseDto
    {
        public string Token { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public DateTime ExpirationDate { get; set; }
    }
}
