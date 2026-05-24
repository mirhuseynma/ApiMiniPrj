
namespace ApiMiniPrj.Persistence.Services
{
    public class AuthService : IAuthService
    {
        private readonly IJwtService _jwtService;
        private readonly UserManager<AppUser> _userManager;
        private readonly JwtSetting jwtSettings;
        private readonly AppDbContext _context;

        public AuthService(IJwtService jwtService, UserManager<AppUser> userManager, IOptions<JwtSetting> options, AppDbContext appDbContext)
        {
            _jwtService = jwtService;
            _userManager = userManager;
            jwtSettings = options.Value;
            _context = appDbContext;
        }
        public async Task<ResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = (await _userManager.FindByEmailAsync(loginDto.EmailOrUsername) ?? await _userManager.FindByNameAsync(loginDto.EmailOrUsername)) ?? throw new ArgumentException("User not found.");
            if (user is not null && !await _userManager.CheckPasswordAsync(user, loginDto.Password)) throw new ArgumentException("Invalid email/username or password.");
            if (!await _userManager.IsEmailConfirmedAsync(user!)) throw new ArgumentException("Email not confirmed. Please confirm your email before logging in.");
            var token = await _jwtService.GenerateTokenAsync(user!);
            var refreshToken = await _jwtService.GenerateRefreshTokenAsync();
            await _context.Set<RefreshToken>().AddAsync(new RefreshToken
            {
                UserId = user!.Id,
                Token = refreshToken,
                Expires = DateTime.UtcNow.AddDays(15),
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
            return new ResponseDto
            {
                Token = token,
                ExpirationDate = DateTime.UtcNow.AddMinutes(jwtSettings.ExpirationMinutes),
                RefreshToken = refreshToken
            };
        }

        public async Task<string> RegisterAsync(RegisterDto registerDto)
        {


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
                // Collect error messages and throw an exception
                throw new ArgumentException("User registration failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            await _userManager.AddToRoleAsync(user, "User");

            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            confirmationToken = WebUtility.UrlEncode(confirmationToken);
            return confirmationToken;
        }

        public async Task ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto)
        {
            var user = await _userManager.FindByEmailAsync(confirmEmailDto.Email) ?? throw new ArgumentException("User not found.");
            var decodedToken = WebUtility.UrlDecode(confirmEmailDto.Token);
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (!result.Succeeded)
            {
                throw new ArgumentException("Email confirmation failed. Invalid token.");
            }
        }

        public async Task<string> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                throw new ArgumentException("User not found or email not confirmed.");
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebUtility.UrlEncode(token);
            return token;
        }

        public async Task ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email) ?? throw new ArgumentException("User not found.");
            if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmPassword)
            {
                throw new ArgumentException("New password and confirm password do not match.");
            }

            var decodedToken = WebUtility.UrlDecode(resetPasswordDto.Token);
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ArgumentException($"Password reset failed: {errors}");
            }
        }

        public async Task<ResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenRequestDto)
        {
            var oldRefreshToken = await _context.Set<RefreshToken>().Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.Token == refreshTokenRequestDto.RefreshToken) ?? throw new ArgumentException("Invalid refresh token.");
            var now = DateTime.UtcNow;

            if (oldRefreshToken.IsExpired || oldRefreshToken.Expires <= now)
            {
                _context.Set<RefreshToken>().Remove(oldRefreshToken);
                await _context.SaveChangesAsync();
                throw new ArgumentException("Refresh token has expired.");
            }

            var newJwtToken = await _jwtService.GenerateTokenAsync(oldRefreshToken.User!);
            var newRefreshToken = await _jwtService.GenerateRefreshTokenAsync();

            var userRefreshTokens = await _context.Set<RefreshToken>()
                .Where(rt => rt.UserId == oldRefreshToken.UserId)
                .ToListAsync();

            _context.Set<RefreshToken>().RemoveRange(userRefreshTokens);

            await _context.Set<RefreshToken>().AddAsync(new RefreshToken
            {
                UserId = oldRefreshToken.UserId,
                Token = newRefreshToken,
                Expires = now.AddDays(15),
                CreatedAt = now
            });

            await _context.SaveChangesAsync();

            return new ResponseDto
            {
                Token = newJwtToken,
                ExpirationDate = now.AddMinutes(jwtSettings.ExpirationMinutes),
                RefreshToken = newRefreshToken
            };
        }
    }
}
