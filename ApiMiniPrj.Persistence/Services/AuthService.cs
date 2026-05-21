using ApiMiniPrj.Application.DTOs.Auth;
using ApiMiniPrj.Application.Interfaces.Auth;
using ApiMiniPrj.Application.Interfaces.JWT;

namespace ApiMiniPrj.Persistence.Services
{
    public class AuthService : IAuthService
    {
        private readonly IJwtService _jwtService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IValidator<LoginDto> _loginValidator;
        private readonly IValidator<RegisterDto> _registerValidator;

        public AuthService(IJwtService jwtService, UserManager<AppUser> userManager, IValidator<LoginDto> loginValidator, IValidator<RegisterDto> registerValidator)
        {
            _jwtService = jwtService;
            _userManager = userManager;
            _loginValidator = loginValidator;
            _registerValidator = registerValidator;
        }
        public async Task<ResponseDto> LoginAsync(LoginDto loginDto)
        {
            var validationResult = _loginValidator.Validate(loginDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException(errors);
            }

            var user = await _userManager.FindByEmailAsync(loginDto.EmailOrUsername) ?? await _userManager.FindByNameAsync(loginDto.EmailOrUsername);
            if (user is null ) throw new ArgumentException("User not found.");
            if (user is not null && !await _userManager.CheckPasswordAsync(user, loginDto.Password)) throw new ArgumentException("Invalid email/username or password."); 
            var token = await _jwtService.GenerateTokenAsync(user!);
            return new ResponseDto
            {
                Token = token,
                ExpirationDate = DateTime.UtcNow.AddMinutes(1) 
            };
        }

        public async Task RegisterAsync(RegisterDto registerDto)
        {
            var validationResult = _registerValidator.Validate(registerDto);
            if (!validationResult.IsValid)
            {
                var errors  = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException(errors);
            }

            var user = new AppUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                FirstName = registerDto.FullName.Split(' ')[0],
                LastName = registerDto.FullName.Split(' ').Length > 1 ? registerDto.FullName.Split(' ')[1] : string.Empty
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ArgumentException(errors);
            }
            await _userManager.AddToRoleAsync(user, "User");    
        }
    }
}
