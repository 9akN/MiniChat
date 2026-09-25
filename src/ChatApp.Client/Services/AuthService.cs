using ChatApp.Client.Models;
using ChatApp.Client.Services.Interfaces;

namespace ChatApp.Client.Services;

public class AuthService(IApiService apiService, ISignalRService signalRService) : IAuthService
{
	private const string TokenKey = "auth_token";
	private const string UserIdKey = "auth_user_id";
	private const string UsernameKey = "auth_username";
	private const string DisplayNameKey = "auth_display_name";
	private const string AvatarUrlKey = "auth_avatar_url";

	public UserModel? CurrentUser { get; private set; }

	public bool IsAuthenticated => CurrentUser is not null;

	public async Task<bool> TryRestoreSessionAsync()
	{
		var token = await SecureStorage.Default.GetAsync(TokenKey);
		var userIdText = await SecureStorage.Default.GetAsync(UserIdKey);

		if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userIdText))
			return false;

		CurrentUser = new UserModel
		{
			Id = int.Parse(userIdText),
			Username = await SecureStorage.Default.GetAsync(UsernameKey) ?? string.Empty,
			DisplayName = await SecureStorage.Default.GetAsync(DisplayNameKey) ?? string.Empty,
			AvatarUrl = await SecureStorage.Default.GetAsync(AvatarUrlKey),
		};

		apiService.AuthToken = token;
		await signalRService.ConnectAsync(token);
		return true;
	}

	public async Task RegisterAsync(string username, string password, string displayName)
	{
		var response = await apiService.RegisterAsync(username, password, displayName);
		await PersistSessionAsync(response);
	}

	public async Task LoginAsync(string username, string password)
	{
		var response = await apiService.LoginAsync(username, password);
		await PersistSessionAsync(response);
	}

	public async Task LogoutAsync()
	{
		await signalRService.DisconnectAsync();
		apiService.AuthToken = null;
		CurrentUser = null;
		SecureStorage.Default.RemoveAll();
	}

	private async Task PersistSessionAsync(AuthResponseModel response)
	{
		CurrentUser = response.User;
		apiService.AuthToken = response.Token;

		await SecureStorage.Default.SetAsync(TokenKey, response.Token);
		await SecureStorage.Default.SetAsync(UserIdKey, response.User.Id.ToString());
		await SecureStorage.Default.SetAsync(UsernameKey, response.User.Username);
		await SecureStorage.Default.SetAsync(DisplayNameKey, response.User.DisplayName);
		await SecureStorage.Default.SetAsync(AvatarUrlKey, response.User.AvatarUrl ?? string.Empty);

		await signalRService.ConnectAsync(response.Token);
	}
}
