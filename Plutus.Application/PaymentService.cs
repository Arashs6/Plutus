namespace Plutus.Application;

public class PaymentService : ICommandHandler<PaymentOrderCreatedCommand>
{
	private readonly IPaymentGatewayFactory _gatewayFactory;
	private readonly ISettlementService _settlementService;

	public PaymentService (IPaymentGatewayFactory gatewayFactory, ISettlementService settlementService)
	{
		_gatewayFactory = gatewayFactory;
		_settlementService = settlementService;
	}

	public async Task Handle (PaymentOrderCreatedCommand command)
	{
		var paymentGateway =  _gatewayFactory.CreatePaymentGateway(command.Type);
		var payment = await paymentGateway.Pay(command);
		if (payment.Status == PaymentStatus.Success)
		{
			var settlementResult = await _settlementService.Settle(command);
			if (settlementResult.Status == PaymentStatus.Success)
			{
				///
			}
		}
	}
}


public interface ISettlementService
{
	Task<PaymentResult> Settle (PaymentOrderCreatedCommand command);
}

public class PaymentResult
{
	public PaymentStatus Status { get; set; }
}

public enum PaymentStatus
{
	Success,
	Failed
}

public interface IPaymentGateway
{
	public GatewayType GatewayType { get;  }
	Task<PaymentResult> Pay (PaymentOrderCreatedCommand command);
}

public class MellatGateway : IPaymentGateway
{
	public GatewayType GatewayType => GatewayType.Mellat;

	public Task<PaymentResult> Pay (PaymentOrderCreatedCommand command)
	{
		throw new NotImplementedException();
	}
}
public class SamanGateway : IPaymentGateway
{
	public GatewayType GatewayType => GatewayType.Saman;

	public Task<PaymentResult> Pay (PaymentOrderCreatedCommand command)
	{
		throw new NotImplementedException();
	}
}
public class PasargadGateway : IPaymentGateway
{
	public GatewayType GatewayType => GatewayType.Pasargad;

	public Task<PaymentResult> Pay (PaymentOrderCreatedCommand command)
	{
		throw new NotImplementedException();
	}
}

public interface IPaymentGatewayFactory
{
	public IPaymentGateway CreatePaymentGateway(GatewayType type);
}
public class PaymentGatewayFactory : IPaymentGatewayFactory
{
	private readonly Dictionary<GatewayType, IPaymentGateway> _gateWays;

	public PaymentGatewayFactory()
	{
		_gateWays = new Dictionary<GatewayType, IPaymentGateway>()
		{
			{ GatewayType.Mellat, new MellatGateway() }
		};
	}
	public IPaymentGateway CreatePaymentGateway(GatewayType type)
	{
		return _gateWays[type];
	}
}