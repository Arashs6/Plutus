namespace Plutus.Application.Gateway;

public interface IPaymentGateway
{
    static abstract GatewayType GatewayType { get; }

    Task<PaymentRequestResult> RequestPaymentAsync(
        long amount,
        string currency,
        string description,
        CancellationToken cancellationToken = default);

    Task<PaymentVerifyResult> VerifyPaymentAsync(
        long amount,
        string reference,
        CancellationToken cancellationToken = default);
}

public sealed record PaymentRequestResult(bool IsSuccess, int Code, string? Reference, string? PaymentUrl);
public sealed record PaymentVerifyResult(bool IsSuccess, int Code, long? RefId);