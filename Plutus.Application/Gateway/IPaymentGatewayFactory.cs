namespace Plutus.Application.Gateway;

public interface IPaymentGatewayFactory
{
	public IPaymentGateway CreatePaymentGateway(GatewayType type);
}