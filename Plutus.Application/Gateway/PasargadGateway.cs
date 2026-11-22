namespace Plutus.Application.Gateway;

public class PasargadGateway : IPaymentGateway
{
    public static GatewayType GatewayType => GatewayType.Pasargad;

    public Task<PaymentRequestResult> RequestPaymentAsync(long amount, string currency, string description, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<PaymentVerifyResult> VerifyPaymentAsync(long amount, string reference, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}