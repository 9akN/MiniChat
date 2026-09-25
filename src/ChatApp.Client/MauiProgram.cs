using ChatApp.Client.Services;
using ChatApp.Client.Services.Interfaces;
using ChatApp.Client.Services.Navigation;
using ChatApp.Client.ViewModels;
using ChatApp.Client.Views;
using Microsoft.Extensions.Logging;

namespace ChatApp.Client;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddSingleton<IApiService, ApiService>();
		builder.Services.AddSingleton<ISignalRService, SignalRService>();
		builder.Services.AddSingleton<IAuthService, AuthService>();
		builder.Services.AddSingleton<INavigationService, MauiNavigationService>();
		builder.Services.AddSingleton<AppShell>();

		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<RegisterViewModel>();
		builder.Services.AddTransient<RegisterPage>();
		builder.Services.AddTransient<ChatsViewModel>();
		builder.Services.AddTransient<ChatsPage>();
		builder.Services.AddTransient<ContactsViewModel>();
		builder.Services.AddTransient<ContactsPage>();
		builder.Services.AddTransient<SettingsViewModel>();
		builder.Services.AddTransient<SettingsPage>();
		builder.Services.AddTransient<ChatViewModel>();
		builder.Services.AddTransient<ChatPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
