namespace ApiMiniPrj.Api.Test.Controllers.Users;

public class UserControllerTests
{
    [Fact]
    public async Task GetAllUsers_WithoutUserIdClaim_ShouldReturnUnauthorized()
    {
        var controller = CreateController();

        var response = await controller.GetAllUsers();

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(response);
        Assert.Equal("User ID not found in token", unauthorized.Value);
    }

    private static UserController CreateController(IValidator<UserUpdateDto>? validator = null)
    {
        var controller = new UserController(
            new RecordingUserService(),
            userManager: null!,
            validator ?? new StubValidator<UserUpdateDto>());

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            }
        };

        return controller;
    }

    private sealed class RecordingUserService : IUserService
    {
        public Task DeleteAsync(string email) => Task.CompletedTask;

        public Task<List<UserGetDto>> GetAllAsync() => Task.FromResult(new List<UserGetDto>());

        public Task<UserGetDto> GetByEmailAsync(string email) => Task.FromResult(new UserGetDto { Email = email });

        public Task UpdateAsync(string email, UserUpdateDto userUpdateDto) => Task.CompletedTask;
    }
}
