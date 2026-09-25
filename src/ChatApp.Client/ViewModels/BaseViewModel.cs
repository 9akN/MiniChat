using CommunityToolkit.Mvvm.ComponentModel;

namespace ChatApp.Client.ViewModels;

public partial class BaseViewModel : ObservableObject
{
	[ObservableProperty]
	private bool isBusy;

	[ObservableProperty]
	private string title = string.Empty;

	[ObservableProperty]
	private string? errorMessage;
}
