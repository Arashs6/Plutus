namespace Plutus.Domain;

/// <summary>
/// Aggregate root representing a payment.
/// </summary>
public class Payment
{
    private Payment()
    {
        Money = null!;
    }

    public Payment(Money money, TimeProvider? timeProvider = null)
    {

        Id = Guid.CreateVersion7();
        Money = money ?? throw new DomainException("Money cannot be null.");
        CreatedAt = (timeProvider ?? TimeProvider.System).GetUtcNow();
    }

    public Guid Id { get; private set; }
    public Money Money { get; private set; }
    public DateTimeOffset CreatedAt { get; init; }
}
public class DomainException(string message) : Exception(message)
{
}
