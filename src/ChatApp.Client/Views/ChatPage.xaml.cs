using ChatApp.Client.ViewModels;

namespace ChatApp.Client.Views;

public partial class ChatPage : ContentPage
{
	private readonly ChatViewModel _viewModel;

	public ChatPage(ChatViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		_viewModel.LoadCommand.Execute(null);
	}
}
