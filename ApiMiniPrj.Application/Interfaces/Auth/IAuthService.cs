namespace ApiMiniPrj.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task RegisterAsync (RegisterDto registerDto);
        Task<ResponseDto> LoginAsync(LoginDto loginDto);
    }
}
