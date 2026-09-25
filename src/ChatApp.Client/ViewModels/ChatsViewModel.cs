using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using ChatApp.Client.Models;
using ChatApp.Client.Services.Interfaces;

namespace ChatApp.Client.ViewModels;

public partial class ChatsViewModel : BaseViewModel
{
	private readonly IApiService _apiService;
	private readonly ISignalRService _signalRService;
	private readonly IAuthService _authService;

	public ChatsViewModel(IApiService apiService, ISignalRService signalRService, IAuthService authService)
	{
		_apiService = apiService;
		_signalRService = signalRService;
		_authService = authService;
		Title = "Chats";
		_signalRService.MessageReceived += OnMessageReceived;
	}

	public ObservableCollection<ConversationModel> Conversations { get; } = [];

	[RelayCommand]
	private async Task LoadAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;
			ErrorMessage = null;
			var conversations = await _apiService.GetConversationsAsync();

			var currentUserId = _authService.CurrentUser?.Id ?? 0;

			Conversations.Clear();
			foreach (var conversation in conversations.OrderByDescending(c => c.LastMessage?.SentAt ?? c.CreatedAt))
			{
				conversation.OtherParticipantsDisplay = string.Join(", ",
					conversation.Participants.Where(p => p.Id != currentUserId).Select(p => p.DisplayName));
				Conversations.Add(conversation);
			}
		}
		catch (Exception ex)
		{
			ErrorMessage = "Could not load chats: " + ex.Message;
		}
		finally
		{
			IsBusy = false;
		}
	}

	private void OnMessageReceived(MessageModel message)
	{
		var conversation = Conversations.FirstOrDefault(c => c.Id == message.ConversationId);
		if (conversation is null)
			return;

		conversation.LastMessage = message;

		var index = Conversations.IndexOf(conversation);
		Conversations.Move(index, 0);
	}
}
