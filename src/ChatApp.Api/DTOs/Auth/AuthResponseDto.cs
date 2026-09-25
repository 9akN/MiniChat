using ChatApp.Api.DTOs.Users;

namespace ChatApp.Api.DTOs.Auth;

public class AuthResponseDto
{
	public required string Token { get; set; }
	public DateTime ExpiresAt { get; set; }
	public required UserDto User { get; set; }
}
