using System.ComponentModel.DataAnnotations;

namespace ChatApp.Api.DTOs.Auth;

public class LoginRequestDto
{
	[Required]
	public required string Username { get; set; }

	[Required]
	public required string Password { get; set; }
}
