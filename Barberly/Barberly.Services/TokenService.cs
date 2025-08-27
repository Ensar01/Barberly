using Barberly.Database;
using Barberly.Interfaces;
using Barberly.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Barberly.Services
{
    public class TokenService : ITokenService
    {
        private readonly DbContext _context;
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;

        public TokenService(DbContext context,IConfiguration config, SymmetricSecurityKey key)
        {
            _context = context;
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]));
        }

        public string GenerateToken(UserTokenDto user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username)
            };
            claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
            var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = credentials,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
