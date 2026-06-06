
namespace ApiMiniPrj.Api.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IValidator<RegisterDto> _registerValidator;
        private readonly IValidator<LoginDto> _loginValidator;
        private readonly IValidator<ConfirmEmailDto> _confirmEmailValidator;
        private readonly IValidator<ForgotPasswordDto> _forgotPasswordValidator;
        private readonly IValidator<ResetPasswordDto> _resetPasswordValidator;

        public AuthController(IAuthService authService, IValidator<LoginDto> loginValidator, IValidator<RegisterDto> registerValidator, IValidator<ConfirmEmailDto> confirmEmailValidator, IValidator<ForgotPasswordDto> forgotPasswordValidator, IValidator<ResetPasswordDto> resetPasswordValidator)
        {
            _authService = authService;
            _loginValidator = loginValidator;
            _registerValidator = registerValidator;
            _confirmEmailValidator = confirmEmailValidator;
            _forgotPasswordValidator = forgotPasswordValidator;
            _resetPasswordValidator = resetPasswordValidator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var validationResult = await _registerValidator.ValidateAsync(registerDto);
            if (!validationResult.IsValid) return ApiResponseFactory.ValidationError(validationResult, HttpContext);

            RegisterResponseDto responseDto = new();
            responseDto.Token = await _authService.RegisterAsync(registerDto);
            return Ok(responseDto);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
        {
            var validationResult = await _loginValidator.ValidateAsync(loginDto);
            if (!validationResult.IsValid) return ApiResponseFactory.ValidationError(validationResult, HttpContext);

            var response = await _authService.LoginAsync(loginDto);
            return Ok(response);
        }


        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromForm] ConfirmEmailDto confirmEmailDto)
        {
            var validationResult = await _confirmEmailValidator.ValidateAsync(confirmEmailDto);
            if (!validationResult.IsValid) return ApiResponseFactory.ValidationError(validationResult, HttpContext);

            await _authService.ConfirmEmailAsync(confirmEmailDto);
            return Ok("Email confirmed successfully.");
        }


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromForm] ForgotPasswordDto forgotPasswordDto)
        {
            var validationResult = await _forgotPasswordValidator.ValidateAsync(forgotPasswordDto);
            if (!validationResult.IsValid) return ApiResponseFactory.ValidationError(validationResult, HttpContext);
            

            var result = await _authService.ForgotPasswordAsync(forgotPasswordDto);

            return Ok(result);

        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordDto resetPasswordDto)
        {
            var validationResult = await _resetPasswordValidator.ValidateAsync(resetPasswordDto);
            if (!validationResult.IsValid) return ApiResponseFactory.ValidationError(validationResult, HttpContext);
            

            await _authService.ResetPasswordAsync(resetPasswordDto);

            return Ok("Password reset successful.");
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromForm] RefreshTokenDto refreshTokenRequestDto)
        {
            var response = await _authService.RefreshTokenAsync(refreshTokenRequestDto);
            return Ok(response);
        }
    }
}
