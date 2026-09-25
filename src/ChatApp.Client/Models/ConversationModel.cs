namespace ChatApp.Client.Models;

public class ConversationModel
{
	public int Id { get; set; }
	public DateTime CreatedAt { get; set; }
	public List<UserModel> Participants { get; set; } = [];
	public MessageModel? LastMessage { get; set; }
	public int UnreadCount { get; set; }

	public string OtherParticipantsDisplay { get; set; } = string.Empty;
}
