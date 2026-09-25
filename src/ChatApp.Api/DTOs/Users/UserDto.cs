namespace ChatApp.Api.DTOs.Users;

public class UserDto
{
	public int Id { get; set; }
	public required string Username { get; set; }
	public required string DisplayName { get; set; }
	public string? AvatarUrl { get; set; }
}
