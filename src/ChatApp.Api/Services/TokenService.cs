using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChatApp.Api.Models;
using ChatApp.Api.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace ChatApp.Api.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
	public (string Token, DateTime ExpiresAt) GenerateToken(User user)
	{
		var jwtSection = configuration.GetSection("Jwt");
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
		var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
		var expiresAt = DateTime.UtcNow.AddMinutes(double.Parse(jwtSection["ExpiryMinutes"]!));

		var claims = new List<Claim>
		{
			new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
			new(JwtRegisteredClaimNames.UniqueName, user.Username),
			new(ClaimTypes.NameIdentifier, user.Id.ToString()),
		};

		var token = new JwtSecurityToken(
			issuer: jwtSection["Issuer"],
			audience: jwtSection["Audience"],
			claims: claims,
			expires: expiresAt,
			signingCredentials: credentials);

		return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
	}
}
