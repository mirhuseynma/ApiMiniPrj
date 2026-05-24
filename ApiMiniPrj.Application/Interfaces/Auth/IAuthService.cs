
namespace ApiMiniPrj.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<string> RegisterAsync (RegisterDto registerDto);
        Task<ResponseDto> LoginAsync(LoginDto loginDto);
        Task ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto);
        Task<string> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<ResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenRequestDto);
    }
}
