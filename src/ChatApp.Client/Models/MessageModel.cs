namespace ChatApp.Client.Models;

public class MessageModel
{
	public int Id { get; set; }
	public int ConversationId { get; set; }
	public int SenderId { get; set; }
	public string SenderDisplayName { get; set; } = string.Empty;
	public string Text { get; set; } = string.Empty;
	public DateTime SentAt { get; set; }
	public bool IsRead { get; set; }

	public bool IsMine { get; set; }
}
