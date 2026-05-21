using ApiMiniPrj.Application.Common.Settings;
using ApiMiniPrj.Application.Interfaces.JWT;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiMiniPrj.Persistence.Services
{
    public class JwtService : IJwtService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly JwtSetting _jwtSetting;

        public JwtService(UserManager<AppUser> userManager, IOptions<JwtSetting> jwtSetting)
        {
            _userManager = userManager;
            _jwtSetting = jwtSetting.Value;
        }
        public async Task<string> GenerateTokenAsync(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("FullName", user.FullName!),
            };

            foreach (var role in roles) claims.Add(new Claim(ClaimTypes.Role, role));
            
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.SecretKey));

            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddMinutes(_jwtSetting.ExpirationMinutes);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiration,
                SigningCredentials = signingCredentials,
                Issuer = _jwtSetting.Issuer,
                Audience = _jwtSetting.Audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);


        }
    }
}
