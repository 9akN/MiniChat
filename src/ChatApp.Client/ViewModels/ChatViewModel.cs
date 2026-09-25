using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ChatApp.Client.Models;
using ChatApp.Client.Services.Interfaces;
using ChatApp.Client.Services.Navigation;

namespace ChatApp.Client.ViewModels;

public partial class ChatViewModel : BaseViewModel, IQueryAttributable, IConfirmNavigation
{
	private readonly IApiService _apiService;
	private readonly ISignalRService _signalRService;
	private readonly IAuthService _authService;

	public ChatViewModel(IApiService apiService, ISignalRService signalRService, IAuthService authService)
	{
		_apiService = apiService;
		_signalRService = signalRService;
		_authService = authService;
		Title = "Chat";
		_signalRService.MessageReceived += OnMessageReceived;
	}

	[ObservableProperty]
	private int conversationId;

	[ObservableProperty]
	private string messageText = string.Empty;

	public ObservableCollection<MessageModel> Messages { get; } = [];

	public int CurrentUserId => _authService.CurrentUser?.Id ?? 0;

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		if (query.TryGetValue("conversationId", out var value))
			ConversationId = Convert.ToInt32(value);
	}

	public Task<bool> CanNavigateFromAsync()
	{
		return string.IsNullOrWhiteSpace(MessageText)
			? Task.FromResult(true)
			: Shell.Current.DisplayAlertAsync("Discard message?", "You have an unsent message. Leave this chat anyway?", "Leave", "Stay");
	}

	[RelayCommand]
	private async Task LoadAsync()
	{
		if (IsBusy || ConversationId == 0)
			return;

		try
		{
			IsBusy = true;
			ErrorMessage = null;

			await _signalRService.JoinConversationAsync(ConversationId);

			var messages = await _apiService.GetMessagesAsync(ConversationId);
			Messages.Clear();
			foreach (var message in messages)
			{
				message.IsMine = message.SenderId == CurrentUserId;
				Messages.Add(message);
			}

			await _apiService.MarkAsReadAsync(ConversationId);
			await _signalRService.MarkAsReadAsync(ConversationId);
		}
		catch (Exception ex)
		{
			ErrorMessage = "Could not load messages: " + ex.Message;
		}
		finally
		{
			IsBusy = false;
		}
	}

	[RelayCommand]
	private async Task SendAsync()
	{
		if (string.IsNullOrWhiteSpace(MessageText) || ConversationId == 0)
			return;

		var text = MessageText.Trim();
		MessageText = string.Empty;

		try
		{
			await _signalRService.SendMessageAsync(ConversationId, text);
		}
		catch (Exception ex)
		{
			ErrorMessage = "Could not send message: " + ex.Message;
		}
	}

	private void OnMessageReceived(MessageModel message)
	{
		if (message.ConversationId != ConversationId)
			return;

		message.IsMine = message.SenderId == CurrentUserId;

		if (MainThread.IsMainThread)
			Messages.Add(message);
		else
			MainThread.BeginInvokeOnMainThread(() => Messages.Add(message));
	}
}
