using ChatApp.Client.Services.Navigation;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.Client.Behaviors;

public class NavigateBehavior : Behavior<View>
{
	public static readonly BindableProperty RouteProperty =
		BindableProperty.Create(nameof(Route), typeof(string), typeof(NavigateBehavior));

	public static readonly BindableProperty ParameterNameProperty =
		BindableProperty.Create(nameof(ParameterName), typeof(string), typeof(NavigateBehavior));

	public static readonly BindableProperty ParameterProperty =
		BindableProperty.Create(nameof(Parameter), typeof(object), typeof(NavigateBehavior));

	public string? Route
	{
		get => (string?)GetValue(RouteProperty);
		set => SetValue(RouteProperty, value);
	}

	public string? ParameterName
	{
		get => (string?)GetValue(ParameterNameProperty);
		set => SetValue(ParameterNameProperty, value);
	}

	public object? Parameter
	{
		get => GetValue(ParameterProperty);
		set => SetValue(ParameterProperty, value);
	}

	private readonly TapGestureRecognizer _tapGestureRecognizer = new();
	private View? _attachedView;

	protected override void OnAttachedTo(View bindable)
	{
		base.OnAttachedTo(bindable);
		_attachedView = bindable;

		BindingContext = bindable.BindingContext;
		bindable.BindingContextChanged += OnBindingContextChanged;

		_tapGestureRecognizer.Tapped += OnTapped;
		bindable.GestureRecognizers.Add(_tapGestureRecognizer);
	}

	protected override void OnDetachingFrom(View bindable)
	{
		bindable.GestureRecognizers.Remove(_tapGestureRecognizer);
		_tapGestureRecognizer.Tapped -= OnTapped;
		bindable.BindingContextChanged -= OnBindingContextChanged;
		_attachedView = null;
		base.OnDetachingFrom(bindable);
	}

	private void OnBindingContextChanged(object? sender, EventArgs e) => BindingContext = _attachedView?.BindingContext;

	private async void OnTapped(object? sender, TappedEventArgs e)
	{
		if (string.IsNullOrEmpty(Route))
			return;

		var navigationService = Application.Current?.Handler?.MauiContext?.Services.GetService<INavigationService>();
		if (navigationService is null)
			return;

		if (Parameter is not null && !string.IsNullOrEmpty(ParameterName))
			await navigationService.GoToAsync(Route, new Dictionary<string, object> { [ParameterName] = Parameter });
		else
			await navigationService.GoToAsync(Route);
	}
}
