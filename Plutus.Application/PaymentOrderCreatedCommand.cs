namespace Plutus.Application;

public class PaymentOrderCreatedCommand : ICommand
{
	public Guid OrderId { get; set; }
	public decimal Amount { get; set; }
	public GatewayType Type { get; set; }
}

public enum GatewayType
{
	Mellat,
	Saman,
	Pasargad
}