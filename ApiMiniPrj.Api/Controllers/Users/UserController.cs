namespace ApiMiniPrj.Api.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IValidator<UserUpdateDto> _validator;

        public UserController(IUserService userService, UserManager<AppUser> userManager, IValidator<UserUpdateDto> validator)
        {
            _userService = userService;
            _userManager = userManager;
            _validator = validator;
        }
        [HttpGet]
        [Authorize(Policy = "Permissions.Users.View")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();
            if (users == null) return NotFound("No users found");
            return Ok(users);
        }

        [HttpGet("{email}")]
        [Authorize(Policy = "Permissions.Users.View")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userService.GetByEmailAsync(email);
            if (user == null)
                return NotFound("User not found");
            return Ok(user);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateUser(string email, [FromForm] UserUpdateDto userUpdateDto)
        {
            var validationResult = await _validator.ValidateAsync(userUpdateDto);
            if (!validationResult.IsValid)
                return BadRequest(string.Join("\n", validationResult.Errors.Select(e => e.ErrorMessage)));
            await _userService.UpdateAsync(email, userUpdateDto);
            return Ok("User updated successfully");
        }

        [HttpDelete("email")]
        [Authorize(Policy = "Permissions.Users.Delete")]
        public async Task<IActionResult> DeleteUser([FromQuery] string email)
        {
            if (email == null) return BadRequest("Email is required");
            await _userService.DeleteAsync(email);
            return Ok("User deleted successfully");
        }

        [Authorize(Policy = "Permissions.Users.Edit")]
        [HttpPut("update-for-admin")]
        public async Task<IActionResult> UpdateUserForAdmin(string email, [FromForm] UserUpdateForAdminDto userUpdateForAdminDto)
        {
            await _userService.UpdateForAdminAsync(email, userUpdateForAdminDto);
            return Ok("User updated successfully");
        }
    }
}


