namespace ChatApp.Client.Models;

public class UserModel
{
	public int Id { get; set; }
	public string Username { get; set; } = string.Empty;
	public string DisplayName { get; set; } = string.Empty;
	public string? AvatarUrl { get; set; }
}
