using ChatApp.Api.DTOs.Conversations;

namespace ChatApp.Api.Services.Interfaces;

public interface IConversationService
{
	Task<IReadOnlyList<ConversationDto>> GetForUserAsync(int userId);
	Task<ConversationDto> GetOrCreateAsync(int userId, int otherUserId);
	Task<bool> IsParticipantAsync(int conversationId, int userId);
}
