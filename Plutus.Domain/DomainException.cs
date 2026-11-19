namespace Plutus.Domain;

public class DomainException(string message) : Exception(message)
{
	public static void ThrowIf(bool condition, string message)
	{
		if (condition)
			throw new DomainException(message);
	}

	public static T ThrowIfNull<T>(T? value, string message) where T : class => value ?? throw new DomainException(message);
}
