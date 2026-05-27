
namespace ApiMiniPrj.Application.Test.Validators.Auth;

public class RegisterDtoValidatorTests
{
    private readonly RegisterDtoValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_WithValidDto_ShouldPass()
    {
        var dto = new RegisterDto
        {
            FullName = "Test User",
            UserName = "testuser",
            Email = "test@example.com",
            Password = "Password123",
            RePassword = "Password123",
            AcceptTerms = true
        };

        var result = await _validator.ValidateAsync(dto);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_WithInvalidDto_ShouldReturnExpectedErrors()
    {
        var dto = new RegisterDto
        {
            FullName = "",
            UserName = "ab",
            Email = "invalid-email",
            Password = "short",
            RePassword = "different",
            AcceptTerms = false
        };

        var result = await _validator.ValidateAsync(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterDto.UserName));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterDto.Email));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterDto.Password));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterDto.RePassword));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterDto.AcceptTerms));
    }
}
