namespace ChatApp.Client.Services.Navigation;

public interface IConfirmNavigation
{
	Task<bool> CanNavigateFromAsync();
}
