using Microsoft.Extensions.DependencyInjection;

namespace Plutus.Application.Gateway;

public class PaymentGatewayFactory : IPaymentGatewayFactory
{
	private readonly IServiceProvider _serviceProvider;

	public PaymentGatewayFactory(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public IPaymentGateway CreatePaymentGateway(GatewayType type)
	{
		return (IPaymentGateway)_serviceProvider.GetRequiredKeyedService(typeof(IPaymentGateway), type);
	}
}