using System.ComponentModel.DataAnnotations;

namespace ChatApp.Api.DTOs.Auth;

public class RegisterRequestDto
{
	[Required, MinLength(3), MaxLength(64)]
	public required string Username { get; set; }

	[Required, MinLength(6)]
	public required string Password { get; set; }

	[Required, MaxLength(128)]
	public required string DisplayName { get; set; }
}
