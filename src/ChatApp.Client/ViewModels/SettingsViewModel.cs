using CommunityToolkit.Mvvm.Input;
using ChatApp.Client.Models;
using ChatApp.Client.Services.Interfaces;
using ChatApp.Client.Services.Navigation;

namespace ChatApp.Client.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
	private readonly IAuthService _authService;
	private readonly INavigationService _navigationService;

	public SettingsViewModel(IAuthService authService, INavigationService navigationService)
	{
		_authService = authService;
		_navigationService = navigationService;
		Title = "Settings";
	}

	public UserModel? CurrentUser => _authService.CurrentUser;

	[RelayCommand]
	private async Task LogoutAsync()
	{
		await _authService.LogoutAsync();
		await _navigationService.GoToAsync("//LoginPage");
	}
}
