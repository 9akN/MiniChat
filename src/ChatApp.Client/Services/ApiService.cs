using System.Net.Http.Headers;
using System.Net.Http.Json;
using ChatApp.Client.Models;
using ChatApp.Client.Services.Interfaces;

namespace ChatApp.Client.Services;

public class ApiService : IApiService
{
	private readonly HttpClient _httpClient;
	private string? _authToken;

	public ApiService()
	{
		_httpClient = new HttpClient { BaseAddress = new Uri(ApiConfig.BaseUrl) };
	}

	public string? AuthToken
	{
		get => _authToken;
		set
		{
			_authToken = value;
			_httpClient.DefaultRequestHeaders.Authorization =
				value is null ? null : new AuthenticationHeaderValue("Bearer", value);
		}
	}

	public async Task<AuthResponseModel> RegisterAsync(string username, string password, string displayName) =>
		await SendAsync<AuthResponseModel>(HttpMethod.Post, "api/auth/register",
			new { Username = username, Password = password, DisplayName = displayName });

	public async Task<AuthResponseModel> LoginAsync(string username, string password) =>
		await SendAsync<AuthResponseModel>(HttpMethod.Post, "api/auth/login",
			new { Username = username, Password = password });

	public async Task<List<UserModel>> GetContactsAsync() =>
		await SendAsync<List<UserModel>>(HttpMethod.Get, "api/users");

	public async Task<List<ConversationModel>> GetConversationsAsync() =>
		await SendAsync<List<ConversationModel>>(HttpMethod.Get, "api/conversations");

	public async Task<ConversationModel> CreateOrGetConversationAsync(int otherUserId) =>
		await SendAsync<ConversationModel>(HttpMethod.Post, "api/conversations", new { OtherUserId = otherUserId });

	public async Task<List<MessageModel>> GetMessagesAsync(int conversationId) =>
		await SendAsync<List<MessageModel>>(HttpMethod.Get, $"api/conversations/{conversationId}/messages");

	public async Task MarkAsReadAsync(int conversationId)
	{
		var response = await _httpClient.PutAsync($"api/conversations/{conversationId}/read", null);
		response.EnsureSuccessStatusCode();
	}

	private async Task<T> SendAsync<T>(HttpMethod method, string uri, object? body = null)
	{
		using var request = new HttpRequestMessage(method, uri);
		if (body is not null)
			request.Content = JsonContent.Create(body);

		using var response = await _httpClient.SendAsync(request);
		response.EnsureSuccessStatusCode();

		return await response.Content.ReadFromJsonAsync<T>()
			?? throw new InvalidOperationException($"Empty response body from {uri}.");
	}
}
