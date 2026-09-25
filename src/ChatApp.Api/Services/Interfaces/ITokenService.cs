using ChatApp.Api.Models;

namespace ChatApp.Api.Services.Interfaces;

public interface ITokenService
{
	(string Token, DateTime ExpiresAt) GenerateToken(User user);
}
