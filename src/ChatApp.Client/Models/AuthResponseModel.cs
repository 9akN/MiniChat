namespace ChatApp.Client.Models;

public class AuthResponseModel
{
	public string Token { get; set; } = string.Empty;
	public DateTime ExpiresAt { get; set; }
	public UserModel User { get; set; } = new();
}
