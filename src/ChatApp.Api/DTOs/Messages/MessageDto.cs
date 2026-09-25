namespace ChatApp.Api.DTOs.Messages;

public class MessageDto
{
	public int Id { get; set; }
	public int ConversationId { get; set; }
	public int SenderId { get; set; }
	public required string SenderDisplayName { get; set; }
	public required string Text { get; set; }
	public DateTime SentAt { get; set; }
	public bool IsRead { get; set; }
}
