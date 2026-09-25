using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using ChatApp.Client.Models;
using ChatApp.Client.Services.Interfaces;
using ChatApp.Client.Services.Navigation;

namespace ChatApp.Client.ViewModels;

public partial class ContactsViewModel : BaseViewModel
{
	private readonly IApiService _apiService;
	private readonly INavigationService _navigationService;

	public ContactsViewModel(IApiService apiService, INavigationService navigationService)
	{
		_apiService = apiService;
		_navigationService = navigationService;
		Title = "Contacts";
	}

	public ObservableCollection<UserModel> Contacts { get; } = [];

	[RelayCommand]
	private async Task LoadAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;
			ErrorMessage = null;
			var contacts = await _apiService.GetContactsAsync();

			Contacts.Clear();
			foreach (var contact in contacts)
				Contacts.Add(contact);
		}
		catch (Exception ex)
		{
			ErrorMessage = "Could not load contacts: " + ex.Message;
		}
		finally
		{
			IsBusy = false;
		}
	}

	[RelayCommand]
	private async Task StartChatAsync(UserModel contact)
	{
		try
		{
			IsBusy = true;
			var conversation = await _apiService.CreateOrGetConversationAsync(contact.Id);
			await _navigationService.GoToAsync(nameof(Views.ChatPage), new Dictionary<string, object>
			{
				["conversationId"] = conversation.Id,
			});
		}
		catch (Exception ex)
		{
			ErrorMessage = "Could not start chat: " + ex.Message;
		}
		finally
		{
			IsBusy = false;
		}
	}
}
