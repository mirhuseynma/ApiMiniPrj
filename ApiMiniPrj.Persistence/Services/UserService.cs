
namespace ApiMiniPrj.Persistence.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;


        public UserService(UserManager<AppUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }
        public async Task DeleteAsync(string email)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null) await _userManager.DeleteAsync(existingUser);
            else throw new NotFoundException("User not found.");
        }

        public async Task<List<UserGetDto>> GetAllAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            if (users == null || users.Count == 0) throw new NotFoundException("No users found.");
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
            var user = await _userManager.FindByEmailAsync(email) ?? throw new NotFoundException("User not found.");
            return _mapper.Map<UserGetDto>(user);
        }


        public async Task UpdateAsync(string email, UserUpdateDto userUpdateDto)
        {
            var user = await _userManager.FindByEmailAsync(email) ?? throw new NotFoundException("User not found.");
            user = _mapper.Map(userUpdateDto, user);

            await _userManager.UpdateAsync(user);
        }

        public async Task UpdateForAdminAsync(string email, UserUpdateForAdminDto userUpdateForAdminDto)
        {
            var user = await _userManager.FindByEmailAsync(email) ?? throw new NotFoundException("User not found.");
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
