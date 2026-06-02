
namespace ApiMiniPrj.Api.Test.Controllers.Auth;

public class AuthControllerTests
{
    [Fact]
    public async Task Register_WithValidModel_ShouldReturnConfirmationToken()
    {
        var service = new RecordingAuthService { RegisterToken = "encoded-token" };
        var controller = CreateController(service);
        var dto = new RegisterDto { Email = "test@example.com", UserName = "test", Password = "Password123", RePassword = "Password123", AcceptTerms = true };

        var response = await controller.Register(dto);

        var okResult = Assert.IsType<OkObjectResult>(response);
        var responseDto = Assert.IsType<RegisterResponseDto>(okResult.Value);
        Assert.Equal("encoded-token", responseDto.Token);
        Assert.Same(dto, service.RegisterDto);
    }

    [Fact]
    public async Task Register_WhenServiceThrowsArgumentException_ShouldReturnBadRequest()
    {
        var service = new RecordingAuthService { RegisterException = new ArgumentException("Duplicate email.") };
        var controller = CreateController(service);
        var dto = new RegisterDto { FullName = "Test User", Email = "test@example.com", UserName = "test", Password = "Password123", RePassword = "Password123", AcceptTerms = true };

        var response = await controller.Register(dto);

        var badRequest = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal("Duplicate email.", badRequest.Value);
    }

    [Fact]
    public async Task Login_WithInvalidModel_ShouldReturnBadRequestAndSkipService()
    {
        var service = new RecordingAuthService();
        var controller = CreateController(service, loginValidator: new StubValidator<LoginDto>("Password is required."));

        var response = await controller.Login(new LoginDto());

        var badRequest = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal("Password is required.", badRequest.Value);
        Assert.False(service.LoginCalled);
    }

    [Fact]
    public async Task RefreshToken_ShouldReturnServiceResponse()
    {
        var responseDto = new ResponseDto { Token = "jwt", RefreshToken = "refresh" };
        var service = new RecordingAuthService { RefreshResponse = responseDto };
        var controller = CreateController(service);
        var dto = new RefreshTokenDto { RefreshToken = "old-refresh" };

        var response = await controller.RefreshToken(dto);

        var okResult = Assert.IsType<OkObjectResult>(response);
        Assert.Same(responseDto, okResult.Value);
        Assert.Same(dto, service.RefreshTokenDto);
    }

    private static AuthController CreateController(
        RecordingAuthService service,
        IValidator<LoginDto>? loginValidator = null,
        IValidator<RegisterDto>? registerValidator = null,
        IValidator<ConfirmEmailDto>? confirmEmailValidator = null,
        IValidator<ForgotPasswordDto>? forgotPasswordValidator = null,
        IValidator<ResetPasswordDto>? resetPasswordValidator = null)
    {
        return new AuthController(
            service,
            loginValidator ?? new StubValidator<LoginDto>(),
            registerValidator ?? new StubValidator<RegisterDto>(),
            confirmEmailValidator ?? new StubValidator<ConfirmEmailDto>(),
            forgotPasswordValidator ?? new StubValidator<ForgotPasswordDto>(),
            resetPasswordValidator ?? new StubValidator<ResetPasswordDto>());
    }

    private sealed class RecordingAuthService : IAuthService
    {
        public string RegisterToken { get; set; } = "token";
        public ArgumentException? RegisterException { get; set; }
        public ResponseDto RefreshResponse { get; set; } = new();
        public RegisterDto? RegisterDto { get; private set; }
        public bool LoginCalled { get; private set; }
        public RefreshTokenDto? RefreshTokenDto { get; private set; }

        public Task ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto) => Task.CompletedTask;

        public Task<string> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto) => Task.FromResult("reset-token");

        public Task<ResponseDto> LoginAsync(LoginDto loginDto)
        {
            LoginCalled = true;
            return Task.FromResult(new ResponseDto());
        }

        public Task<string> RegisterAsync(RegisterDto registerDto)
        {
            if (RegisterException is not null)
            {
                throw RegisterException;
            }

            RegisterDto = registerDto;
            return Task.FromResult(RegisterToken);
        }

        public Task ResetPasswordAsync(ResetPasswordDto resetPasswordDto) => Task.CompletedTask;

        public Task<ResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenRequestDto)
        {
            RefreshTokenDto = refreshTokenRequestDto;
            return Task.FromResult(RefreshResponse);
        }
    }
}
