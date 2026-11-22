using Plutus.Application.Commands;

namespace Plutus.Application;

public interface ISettlementService
{
	Task<PaymentResult> Settle (PaymentOrderCreatedCommand command);
}