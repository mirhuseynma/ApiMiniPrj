
namespace ApiMiniPrj.Api.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<AppUser> _userManager;

        public UserController(IUserService userService, UserManager<AppUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized("User ID not found in token");
            var currentUser = await _userManager.FindByIdAsync(userId);
            if (currentUser == null) return NotFound("User not found");

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

            var user = await _userService.GetByEmailAsync(email);
            if (user == null)
                return NotFound("User not found");
            return Ok(user);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser(string email, [FromForm] UserUpdateDto userUpdateDto)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized("User ID not found in token");
            var currentUser = await _userManager.FindByIdAsync(userId);
            if (currentUser == null) return NotFound("User not found");
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
            if (email == null) return BadRequest("Email is required");
            await _userService.DeleteAsync(email);
            return Ok("User deleted successfully");
        }
    }
}
