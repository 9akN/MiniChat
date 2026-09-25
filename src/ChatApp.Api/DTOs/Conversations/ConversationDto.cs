using ChatApp.Api.DTOs.Messages;
using ChatApp.Api.DTOs.Users;

namespace ChatApp.Api.DTOs.Conversations;

public class ConversationDto
{
	public int Id { get; set; }
	public DateTime CreatedAt { get; set; }
	public List<UserDto> Participants { get; set; } = [];
	public MessageDto? LastMessage { get; set; }
	public int UnreadCount { get; set; }
}
