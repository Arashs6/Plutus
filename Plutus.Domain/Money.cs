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
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        ArgumentException.ThrowIfNullOrWhiteSpace(currency);

        Amount = amount;
        Currency = currency.Trim().ToUpperInvariant();
    }

    public decimal Amount { get; }
    public string Currency { get; }

    public static Money Zero(string currency) => new(0m, currency);

   
   
}
