
using ApiMiniPrj.Application.DTOs.Users;
using ApiMiniPrj.Application.Interfaces.User;
using ApiMiniPrj.Domain.Models.Users;
using Microsoft.AspNetCore.Identity;

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
            if (email == null) throw new ArgumentNullException(nameof(email));
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null && await _userManager.IsInRoleAsync(existingUser, "User")) await _userManager.DeleteAsync(existingUser);
            throw new Exception("User not found");
        }

        public async Task<List<UserGetDto>> GetAllAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            if (users == null || users.Count == 0) throw new Exception("No users found");
            var userDtos = _mapper.Map<List<UserGetDto>>(users).ToList();
            return userDtos;
        }

        public async Task<UserGetDto> GetByEmailAsync(string email)
        {
            if (email == null) throw new ArgumentNullException(nameof(email));
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) throw new Exception("User not found");
            return _mapper.Map<UserGetDto>(user);
        }


        public async Task UpdateAsync(string email, UserUpdateDto userUpdateDto)
        {
            if (email == null) throw new ArgumentNullException(nameof(email));

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) throw new Exception("User not found");

            // Validate the update DTO
            var validationResult = await _updateValidator.ValidateAsync(userUpdateDto);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException("Invalid user update data");
            }
            if (userUpdateDto.AssignAdmin)
            {
                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                }
                if (!await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
            }
            user = _mapper.Map(userUpdateDto, user);

            await _userManager.UpdateAsync(user);
        }
    }
}
