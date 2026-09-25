namespace ChatApp.Client.Validation;

public class IsNotNullOrEmptyRule<T> : IValidationRule<T>
{
	public required string ValidationMessage { get; init; }

	public bool Check(T? value) => value switch
	{
		null => false,
		string s => !string.IsNullOrWhiteSpace(s),
		_ => true,
	};
}
