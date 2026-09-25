using ChatApp.Client.Models;

namespace ChatApp.Client.Services.Interfaces;

public interface ISignalRService
{
	event Action<MessageModel>? MessageReceived;
	event Action<int, int>? MessagesRead;

	Task ConnectAsync(string token);
	Task DisconnectAsync();
	Task JoinConversationAsync(int conversationId);
	Task SendMessageAsync(int conversationId, string text);
	Task MarkAsReadAsync(int conversationId);
}
