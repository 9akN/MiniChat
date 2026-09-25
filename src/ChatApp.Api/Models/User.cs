namespace ChatApp.Api.Models;

public class User
{
	public int Id { get; set; }
	public required string Username { get; set; }
	public required string PasswordHash { get; set; }
	public required string DisplayName { get; set; }
	public string? AvatarUrl { get; set; }
	public DateTime CreatedAt { get; set; }

	public ICollection<ConversationUser> ConversationUsers { get; set; } = [];
	public ICollection<Message> Messages { get; set; } = [];
}
