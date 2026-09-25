using ChatApp.Api.DTOs.Messages;

namespace ChatApp.Api.Services.Interfaces;

public interface IMessageService
{
	Task<IReadOnlyList<MessageDto>> GetForConversationAsync(int conversationId);
	Task<MessageDto> SendAsync(int conversationId, int senderId, string text);
	Task MarkAsReadAsync(int conversationId, int readerId);
}
