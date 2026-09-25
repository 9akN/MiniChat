namespace ChatApp.Api.Models;

public class ConversationUser
{
	public int ConversationId { get; set; }
	public Conversation Conversation { get; set; } = null!;

	public int UserId { get; set; }
	public User User { get; set; } = null!;

	public DateTime JoinedAt { get; set; }
}
