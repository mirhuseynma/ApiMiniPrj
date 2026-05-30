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
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();
            if (users == null) return NotFound("No users found");
            return Ok(users);
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userService.GetByEmailAsync(email);
            if (user == null)
                return NotFound("User not found");
            return Ok(user);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser(string email, [FromForm] UserUpdateDto userUpdateDto)
        {
            await _userService.UpdateAsync(email, userUpdateDto);
            return Ok("User updated successfully");
        }

        [HttpDelete("email")]
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



//var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

//if (string.IsNullOrWhiteSpace(userId)) return Unauthorized("User ID not found in token");
//var currentUser = await _userManager.FindByIdAsync(userId);
//if (currentUser == null) return NotFound("User not found");
//if (!await _userManager.IsInRoleAsync(currentUser, "Admin") && currentUser.Email != email)
//    return Forbid("You are not allowed to view other users");