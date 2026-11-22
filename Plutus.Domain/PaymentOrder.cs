namespace Plutus.Domain;

public class PaymentOrder
{
	private List<Payment> _payments = new List<Payment>();
	public PaymentOrder(Money money)
	{
		Money = DomainException.ThrowIfNull(money, "Money cannot be null."); ;
		Id = Guid.NewGuid();
		PaymentStatus = PaymentStatus.UnPaid;
		
		CreatedAt = TimeProvider.System.GetUtcNow();
	}
	public IReadOnlyList<Payment> Payments => _payments.AsReadOnly();
	public Guid Id { get;private set; }
	public PaymentStatus PaymentStatus { get;private set; }
	public Money Money { get;private set; }
	public DateTimeOffset CreatedAt { get; private init; }
	public DateTimeOffset? FinishDate { get;private set; }
	public void AddPayment(Money money)
	{
		DomainException.ThrowIfNull(money, "Amount cannot be null.");
		_payments.Add(new Payment(money));
	}

	public void MarkAsPaid()
	{
		FinishDate = TimeProvider.System.GetUtcNow();
		PaymentStatus = PaymentStatus.Paid;
	}
}