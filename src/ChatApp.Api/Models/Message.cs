namespace ChatApp.Api.Models;

public class Message
{
	public int Id { get; set; }
	public int ConversationId { get; set; }
	public Conversation Conversation { get; set; } = null!;

	public int SenderId { get; set; }
	public User Sender { get; set; } = null!;

	public required string Text { get; set; }
	public DateTime SentAt { get; set; }
	public bool IsRead { get; set; }
}
