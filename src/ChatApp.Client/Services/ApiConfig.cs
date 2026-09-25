namespace ChatApp.Client.Services;

public static class ApiConfig
{
	public static string BaseUrl =>
#if ANDROID
		"http://10.0.2.2:5203";
#else
		"https://localhost:7072";
#endif

	public static string HubUrl => $"{BaseUrl}/hubs/chat";
}
