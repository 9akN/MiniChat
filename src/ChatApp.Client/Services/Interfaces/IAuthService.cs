using ChatApp.Client.Models;

namespace ChatApp.Client.Services.Interfaces;

public interface IAuthService
{
	UserModel? CurrentUser { get; }
	bool IsAuthenticated { get; }

	Task<bool> TryRestoreSessionAsync();
	Task RegisterAsync(string username, string password, string displayName);
	Task LoginAsync(string username, string password);
	Task LogoutAsync();
}
