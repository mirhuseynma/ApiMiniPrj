
namespace ApiMiniPrj.Persistence.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IValidator<UserUpdateDto> _updateValidator;


        public UserService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper, IValidator<UserUpdateDto> updateValidator)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _updateValidator = updateValidator;
        }
        public async Task DeleteAsync(string email)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null) await _userManager.DeleteAsync(existingUser);
            else throw new Exception("User not found");
        }

        public async Task<List<UserGetDto>> GetAllAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            if (users == null || users.Count == 0) throw new Exception("No users found");
            var userDtos = new List<UserGetDto>();
            foreach (var user in users)
            {
                var userDto = _mapper.Map<UserGetDto>(user);
                var roles = await _userManager.GetRolesAsync(user);
                userDto.Roles = roles.ToList();
                userDtos.Add(userDto);
            }
            return userDtos;
        }

        public async Task<UserGetDto> GetByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email) ?? throw new Exception("User not found");
            return _mapper.Map<UserGetDto>(user);
        }


        public async Task UpdateAsync(string email, UserUpdateDto userUpdateDto)
        {
            var user = await _userManager.FindByEmailAsync(email) ?? throw new Exception("User not found");

            // Validate the update DTO
            var validationResult = await _updateValidator.ValidateAsync(userUpdateDto);
            if (!validationResult.IsValid) throw new ArgumentException("Invalid user update data");


            user = _mapper.Map(userUpdateDto, user);

            await _userManager.UpdateAsync(user);
        }

        public async Task UpdateForAdminAsync(string email, UserUpdateForAdminDto userUpdateForAdminDto)
        {
            var user = await _userManager.FindByEmailAsync(email) ?? throw new Exception("User not found");
            if (userUpdateForAdminDto.IsAdmin == false)
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                    await _userManager.RemoveFromRoleAsync(user, "Admin");
                var userRoles = await _userManager.GetRolesAsync(user);
                if (userRoles.Count == 0)
                    await _userManager.AddToRoleAsync(user, "User");
            }
            else await _userManager.AddToRoleAsync(user, "Admin");


            if (userUpdateForAdminDto.IsOrganizer == false)
            {
                if (await _userManager.IsInRoleAsync(user, "Organizer"))
                    await _userManager.RemoveFromRoleAsync(user, "Organizer");
                var userRoles = await _userManager.GetRolesAsync(user);
                if (userRoles.Count == 0)
                    await _userManager.AddToRoleAsync(user, "User");
            }
            else await _userManager.AddToRoleAsync(user, "Organizer");

            user = _mapper.Map(userUpdateForAdminDto, user);
            await _userManager.UpdateAsync(user);
        }
    }
}
