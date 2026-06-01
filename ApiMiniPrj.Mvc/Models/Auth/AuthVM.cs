using ApiMiniPrj.Application.DTOs.Auth;

namespace ApiMiniPrj.Mvc.Models.Auth
{
    public class AuthVM
    {
        public LoginDto Login { get; set; } = new LoginDto();
        public RegisterDto Register { get; set; } = new RegisterDto();
    }
}
