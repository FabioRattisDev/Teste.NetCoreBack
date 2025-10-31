using FullstackApp.Domain.Services.Interface;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FullstackApp.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        public string GenerateToken(Guid userId, string email)
        {
            var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? throw new Exception("JWT__Key env var missing");
            var jwtIssuer = Environment.GetEnvironmentVariable("JWT_Issuer") ?? "default_issuer";
            var jwtAudience = Environment.GetEnvironmentVariable("JWT_Audience") ?? "default_audience";
            var jwtExpiryMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_ExpireMinutes") ?? "60");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim("id", userId.ToString()),
            new Claim("email", email)
        };

            var token = new JwtSecurityToken(
                jwtIssuer,
                jwtAudience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(jwtExpiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
