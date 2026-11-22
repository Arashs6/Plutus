using Plutus.Application.Gateway;

namespace Plutus.Application.Commands;

public class PaymentOrderCreatedCommand : ICommand
{
	public Guid OrderId { get; set; }
	public decimal Amount { get; set; }
	public GatewayType Type { get; set; }
}