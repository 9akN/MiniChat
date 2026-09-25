using System.ComponentModel.DataAnnotations;

namespace ChatApp.Api.DTOs.Conversations;

public class CreateConversationDto
{
	[Required]
	public int OtherUserId { get; set; }
}
