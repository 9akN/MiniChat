namespace ChatApp.Api.Models;

public class Conversation
{
	public int Id { get; set; }
	public DateTime CreatedAt { get; set; }

	public ICollection<ConversationUser> ConversationUsers { get; set; } = [];
	public ICollection<Message> Messages { get; set; } = [];
}
