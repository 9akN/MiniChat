using ChatApp.Client.Services.Interfaces;
using ChatApp.Client.Services.Navigation;
using ChatApp.Client.Views;

namespace ChatApp.Client;

public partial class AppShell : Shell
{
	private readonly IAuthService _authService;
	private readonly INavigationService _navigationService;

	public AppShell(IAuthService authService, INavigationService navigationService)
	{
		InitializeComponent();
		_authService = authService;
		_navigationService = navigationService;

		Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
		Routing.RegisterRoute(nameof(ChatPage), typeof(ChatPage));

		Loaded += OnLoaded;
		Navigating += OnShellNavigating;
	}

	private async void OnLoaded(object? sender, EventArgs e)
	{
		Loaded -= OnLoaded;

		var restored = await _authService.TryRestoreSessionAsync();
		if (restored)
			await _navigationService.GoToAsync("//ChatsPage");
	}

	private void OnShellNavigating(object? sender, ShellNavigatingEventArgs e)
	{
		if (CurrentPage?.BindingContext is not IConfirmNavigation confirm)
			return;

		var deferral = e.GetDeferral();
		ConfirmNavigationAsync(e, confirm, deferral);
	}

	private static async void ConfirmNavigationAsync(ShellNavigatingEventArgs e, IConfirmNavigation confirm, ShellNavigatingDeferral deferral)
	{
		if (!await confirm.CanNavigateFromAsync())
			e.Cancel();

		deferral.Complete();
	}
}
