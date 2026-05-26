
namespace ApiMiniPrj.Api.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
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
        public async Task<IActionResult> GetAllUsers()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized("User ID not found in token");
            var currentUser = await _userManager.FindByIdAsync(userId);
            if (currentUser == null) return NotFound("User not found");
            if (!await _userManager.IsInRoleAsync(currentUser, "Admin"))
                return Forbid("You are not allowed to view other users");
            var users = await _userService.GetAllAsync();
            if (users == null) return NotFound("No users found");
            return Ok(users);
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized("User ID not found in token");
            var currentUser = await _userManager.FindByIdAsync(userId);
            if (currentUser == null) return NotFound("User not found");
            if (!await _userManager.IsInRoleAsync(currentUser, "Admin") && currentUser.Email != email)
                return Forbid("You are not allowed to view other users");
            var user = await _userService.GetByEmailAsync(email);
            if (user == null)
                return NotFound("User not found");
            return Ok(user);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser(string email, [FromForm] UserUpdateDto userUpdateDto)
        {
            var validationResult = await _validator.ValidateAsync(userUpdateDto);
            if (!validationResult.IsValid) return BadRequest(string.Join("\n", validationResult.Errors));
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized("User ID not found in token");
            var currentUser = await _userManager.FindByIdAsync(userId);
            // only admin can update other users, so we check if the current user is admin
            if (currentUser == null) return NotFound("User not found");
            if (!await _userManager.IsInRoleAsync(currentUser, "Admin") && currentUser.Email != email)
                return Forbid("You are not allowed to update other users");
            await _userService.UpdateAsync(email, userUpdateDto);
            return Ok("User updated successfully");
        }
        [HttpDelete("email")]
        public async Task<IActionResult> DeleteUser([FromQuery] string email)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized("User ID not found in token");
            var currentUser = await _userManager.FindByIdAsync(userId);
            if (currentUser == null) return NotFound("User not found");
            if (!await _userManager.IsInRoleAsync(currentUser, "Admin"))
                return Forbid("You are not allowed to delete users");
            if (email == null) return BadRequest("Email is required");
            await _userService.DeleteAsync(email);
            return Ok("User deleted successfully");
        }
    }
}
