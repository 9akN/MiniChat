using ChatApp.Client.Models;

namespace ChatApp.Client.Services.Interfaces;

public interface IApiService
{
	string? AuthToken { get; set; }

	Task<AuthResponseModel> RegisterAsync(string username, string password, string displayName);
	Task<AuthResponseModel> LoginAsync(string username, string password);
	Task<List<UserModel>> GetContactsAsync();
	Task<List<ConversationModel>> GetConversationsAsync();
	Task<ConversationModel> CreateOrGetConversationAsync(int otherUserId);
	Task<List<MessageModel>> GetMessagesAsync(int conversationId);
	Task MarkAsReadAsync(int conversationId);
}
