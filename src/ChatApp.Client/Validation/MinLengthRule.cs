namespace ChatApp.Client.Validation;

public class MinLengthRule : IValidationRule<string>
{
	public required int MinLength { get; init; }

	public required string ValidationMessage { get; init; }

	public bool Check(string? value) => (value?.Length ?? 0) >= MinLength;
}
