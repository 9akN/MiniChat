using System.ComponentModel.DataAnnotations;

namespace ChatApp.Api.DTOs.Messages;

public class SendMessageDto
{
	[Required, MaxLength(4000)]
	public required string Text { get; set; }
}
