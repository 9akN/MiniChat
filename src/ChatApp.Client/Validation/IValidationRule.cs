namespace ChatApp.Client.Validation;

public interface IValidationRule<T>
{
	string ValidationMessage { get; }

	bool Check(T? value);
}
