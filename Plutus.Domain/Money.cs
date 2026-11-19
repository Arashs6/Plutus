namespace Plutus.Domain;

/// <summary>
/// Value object representing a non-negative monetary amount in a specific currency.
/// </summary>
public sealed record Money
{
	// Required by ORMs (e.g., EF Core) for materialization.
	private Money()
	{
		Currency = string.Empty;
	}

	public Money(decimal amount, string currency)
	{
		ThrowForNegativeValue(amount);
		DomainException.ThrowIf(string.IsNullOrWhiteSpace(currency), "Currency cannot be null or whitespace.");

		Amount = amount;
		Currency = currency.Trim().ToUpperInvariant();
	}

	public decimal Amount { get; }

	public string Currency { get; }

	public static Money Zero(string currency) => new(0m, currency);

	public Money Add(Money money)
	{
		ThrowIfMoneyIsNull(money);
		ThrowForMismatchCurrency(money);
		
		return new Money(Amount + money.Amount, Currency);
	}

	public Money Subtract(Money money)
	{
		ThrowIfMoneyIsNull(money);
		ThrowForMismatchCurrency(money);

		var result = Amount - money.Amount;
		DomainException.ThrowIf(result < 0, "Result cannot be negative.");

		return new Money(result, Currency);
	}

	private static void ThrowIfMoneyIsNull(Money money)
	{
		DomainException.ThrowIfNull(money, "Money cannot be null.");
	}

	private void ThrowForMismatchCurrency(Money money)
	{
		DomainException.ThrowIf(money.Currency != Currency, "Currencies must match.");
	}

	private static void ThrowForNegativeValue(decimal amount)
	{
		DomainException.ThrowIf(amount < 0, "Amount cannot be negative.");
	}
}
