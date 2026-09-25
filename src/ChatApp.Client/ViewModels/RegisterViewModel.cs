using CommunityToolkit.Mvvm.Input;
using ChatApp.Client.Services.Interfaces;
using ChatApp.Client.Services.Navigation;
using ChatApp.Client.Validation;

namespace ChatApp.Client.ViewModels;

public partial class RegisterViewModel : BaseViewModel
{
	private readonly IAuthService _authService;
	private readonly INavigationService _navigationService;

	public RegisterViewModel(IAuthService authService, INavigationService navigationService)
	{
		_authService = authService;
		_navigationService = navigationService;
		Title = "Create Account";

		DisplayName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Enter a display name." });
		Username.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Enter a username." });
		Password.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Enter a password." });
		Password.Validations.Add(new MinLengthRule { MinLength = 6, ValidationMessage = "Password must be at least 6 characters." });
	}

	public ValidatableObject<string> DisplayName { get; } = new();
	public ValidatableObject<string> Username { get; } = new();
	public ValidatableObject<string> Password { get; } = new();

	private bool ValidateForm()
	{
		var isDisplayNameValid = DisplayName.Validate();
		var isUsernameValid = Username.Validate();
		var isPasswordValid = Password.Validate();
		return isDisplayNameValid && isUsernameValid && isPasswordValid;
	}

	[RelayCommand]
	private async Task RegisterAsync()
	{
		if (IsBusy)
			return;

		if (!ValidateForm())
			return;

		try
		{
			IsBusy = true;
			ErrorMessage = null;
			await _authService.RegisterAsync(Username.Value!.Trim(), Password.Value!, DisplayName.Value!.Trim());
			await _navigationService.GoToAsync("//ChatsPage");
		}
		catch (Exception ex)
		{
			ErrorMessage = "Registration failed: " + ex.Message;
		}
		finally
		{
			IsBusy = false;
		}
	}
}
