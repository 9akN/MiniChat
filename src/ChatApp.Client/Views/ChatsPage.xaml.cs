using ChatApp.Client.ViewModels;

namespace ChatApp.Client.Views;

public partial class ChatsPage : ContentPage
{
	private readonly ChatsViewModel _viewModel;

	public ChatsPage(ChatsViewModel viewModel)
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
