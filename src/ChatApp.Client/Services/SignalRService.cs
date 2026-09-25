using ChatApp.Client.Models;
using ChatApp.Client.Services.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;

namespace ChatApp.Client.Services;

public class SignalRService : ISignalRService
{
	private HubConnection? _connection;

	public event Action<MessageModel>? MessageReceived;
	public event Action<int, int>? MessagesRead;

	public async Task ConnectAsync(string token)
	{
		if (_connection is not null)
			await DisconnectAsync();

		_connection = new HubConnectionBuilder()
			.WithUrl(ApiConfig.HubUrl, options => options.AccessTokenProvider = () => Task.FromResult<string?>(token))
			.WithAutomaticReconnect()
			.Build();

		_connection.On<MessageModel>("ReceiveMessage", message => MessageReceived?.Invoke(message));
		_connection.On<int, int>("MessagesRead", (conversationId, readerId) => MessagesRead?.Invoke(conversationId, readerId));

		await _connection.StartAsync();
	}

	public async Task DisconnectAsync()
	{
		if (_connection is null)
			return;

		await _connection.DisposeAsync();
		_connection = null;
	}

	public Task JoinConversationAsync(int conversationId) =>
		_connection?.InvokeAsync("JoinConversation", conversationId) ?? Task.CompletedTask;

	public Task SendMessageAsync(int conversationId, string text) =>
		_connection?.InvokeAsync("SendMessage", conversationId, text) ?? Task.CompletedTask;

	public Task MarkAsReadAsync(int conversationId) =>
		_connection?.InvokeAsync("MarkAsRead", conversationId) ?? Task.CompletedTask;
}
