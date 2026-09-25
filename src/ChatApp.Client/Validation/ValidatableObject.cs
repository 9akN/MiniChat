using CommunityToolkit.Mvvm.ComponentModel;

namespace ChatApp.Client.Validation;

public partial class ValidatableObject<T> : ObservableObject
{
	[ObservableProperty]
	private T? value;

	[ObservableProperty]
	private bool isValid = true;

	[ObservableProperty]
	private string error = string.Empty;

	private bool _hasBeenValidated;

	public List<IValidationRule<T>> Validations { get; } = [];

	partial void OnValueChanged(T? value)
	{
		if (_hasBeenValidated)
			Validate();
	}

	public bool Validate()
	{
		_hasBeenValidated = true;

		var failed = Validations.Where(rule => !rule.Check(Value)).ToList();
		Error = failed.FirstOrDefault()?.ValidationMessage ?? string.Empty;
		IsValid = failed.Count == 0;

		return IsValid;
	}
}
