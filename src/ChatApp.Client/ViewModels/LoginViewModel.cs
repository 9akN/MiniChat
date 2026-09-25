using CommunityToolkit.Mvvm.Input;
using ChatApp.Client.Services.Interfaces;
using ChatApp.Client.Services.Navigation;
using ChatApp.Client.Validation;

namespace ChatApp.Client.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
	private readonly IAuthService _authService;
	private readonly INavigationService _navigationService;

	public LoginViewModel(IAuthService authService, INavigationService navigationService)
	{
		_authService = authService;
		_navigationService = navigationService;
		Title = "Log In";

		Username.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Enter your username." });
		Password.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Enter your password." });
	}

	public ValidatableObject<string> Username { get; } = new();
	public ValidatableObject<string> Password { get; } = new();

	private bool ValidateForm()
	{
		var isUsernameValid = Username.Validate();
		var isPasswordValid = Password.Validate();
		return isUsernameValid && isPasswordValid;
	}

	[RelayCommand]
	private async Task LoginAsync()
	{
		if (IsBusy)
			return;

		if (!ValidateForm())
			return;

		try
		{
			IsBusy = true;
			ErrorMessage = null;
			await _authService.LoginAsync(Username.Value!.Trim(), Password.Value!);
			await _navigationService.GoToAsync("//ChatsPage");
		}
		catch (Exception ex)
		{
			ErrorMessage = "Login failed: " + ex.Message;
		}
		finally
		{
			IsBusy = false;
		}
	}
}
