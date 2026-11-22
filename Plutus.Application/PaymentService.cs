using Plutus.Application.Commands;
using Plutus.Application.Gateway;
using Plutus.Domain;

namespace Plutus.Application;

public class PaymentService : ICommandHandler<PaymentOrderCreatedCommand>
{
    private readonly IPaymentGatewayFactory _gatewayFactory;
    private readonly ISettlementService _settlementService;

    public PaymentService(IPaymentGatewayFactory gatewayFactory, ISettlementService settlementService)
    {
        _gatewayFactory = gatewayFactory;
        _settlementService = settlementService;
    }

    public async Task Handle(PaymentOrderCreatedCommand command)
    {
        var paymentGateway = _gatewayFactory.CreatePaymentGateway(command.Type);
        var payment = await paymentGateway.RequestPaymentAsync((long)command.Amount, "IRR", $"Order {command.OrderId}");
        if (payment.IsSuccess)
        {
            var settlementResult = await _settlementService.Settle(command);
            if (settlementResult.Status == PaymentStatus.Paid)
            {
                ///
            }
        }
    }
}