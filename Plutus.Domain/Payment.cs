namespace Plutus.Domain;


public class Payment
{
    private Payment()
    {
        Money = null!;
    }

    public Payment(Money money, TimeProvider? timeProvider = null)
    {
        Money = DomainException.ThrowIfNull(money, "Money cannot be null.");
        DomainException.ThrowIf(money.Amount <= 0, "Payment amount must be greater than zero.");

        Id = Guid.NewGuid();
        CreatedAt = (timeProvider ?? TimeProvider.System).GetUtcNow();
    }

    public Guid Id { get; private set; }
    public Money Money { get; private set; }
    public DateTimeOffset CreatedAt { get; private init; }
}

public enum PaymentStatus
{
	UnPaid,
    Paid,
	Pending,
    Canceled,
	Expired
}
