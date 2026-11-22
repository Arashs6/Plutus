using Plutus.Application.Gateway;
using Plutus.Domain;

namespace Plutus.Application;

public interface IPaymentAttemptStore
{
    void Save(string reference, PaymentOrder order, long amount);
    bool TryGet(string reference, out PaymentAttempt? attempt);
}

public sealed class PaymentAttempt(PaymentOrder order, long amount)
{
    public PaymentOrder Order { get; } = order;
    public long Amount { get; } = amount;
    public long? RefId { get; set; }
}

public sealed class StartPaymentHandler
{
    private readonly IPaymentGatewayFactory _gatewayFactory;
    private readonly IPaymentAttemptStore _store;

    public StartPaymentHandler(IPaymentGatewayFactory gatewayFactory, IPaymentAttemptStore store)
    {
        _gatewayFactory = gatewayFactory;
        _store = store;
    }

    public async Task<StartPaymentResponse> HandleAsync(
        GatewayType gatewayType,
        CancellationToken cancellationToken = default)
    {
        var money = new Money(100_000m, "IRR");
        var order = new PaymentOrder(money);
        order.AddPayment(money);

        var gateway = _gatewayFactory.CreatePaymentGateway(gatewayType);
        var requestResult = await gateway.RequestPaymentAsync(
            (long)money.Amount,
            money.Currency,
            $"پرداخت آزمایشی سفارش {order.Id}",
            cancellationToken);

        if (!requestResult.IsSuccess || requestResult.Reference is null)
        {
            return new StartPaymentResponse(false, order.Id, null, null, requestResult.Code);
        }

        _store.Save(requestResult.Reference, order, (long)money.Amount);
        return new StartPaymentResponse(true, order.Id, requestResult.Reference, requestResult.PaymentUrl, requestResult.Code);
    }
}

public sealed class VerifyPaymentHandler
{
    private readonly IPaymentGatewayFactory _gatewayFactory;
    private readonly IPaymentAttemptStore _store;

    public VerifyPaymentHandler(IPaymentGatewayFactory gatewayFactory, IPaymentAttemptStore store)
    {
        _gatewayFactory = gatewayFactory;
        _store = store;
    }

    public async Task<VerifyPaymentResponse> HandleAsync(
        GatewayType gatewayType,
        string? reference,
        string? status,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(reference) || !_store.TryGet(reference, out var attempt) || attempt is null)
        {
            return new VerifyPaymentResponse(false, true, null, null, null, "Payment attempt not found.");
        }

        if (!string.Equals(status, "OK", StringComparison.OrdinalIgnoreCase))
        {
            return new VerifyPaymentResponse(
                false,
                false,
                attempt.Order.Id,
                attempt.Order.PaymentStatus.ToString(),
                null,
                "Payment was canceled or failed.");
        }

        if (attempt.Order.PaymentStatus == PaymentStatus.Paid)
        {
            return new VerifyPaymentResponse(
                true,
                false,
                attempt.Order.Id,
                attempt.Order.PaymentStatus.ToString(),
                attempt.RefId,
                "Payment already verified.");
        }

        var gateway = _gatewayFactory.CreatePaymentGateway(gatewayType);
        var verifyResult = await gateway.VerifyPaymentAsync(attempt.Amount, reference, cancellationToken);
        if (!verifyResult.IsSuccess)
        {
            return new VerifyPaymentResponse(
                false,
                false,
                attempt.Order.Id,
                attempt.Order.PaymentStatus.ToString(),
                null,
                $"Payment verification failed (code: {verifyResult.Code}).");
        }

        if (verifyResult.Code == 100)
        {
            attempt.Order.MarkAsPaid();
            attempt.RefId = verifyResult.RefId;
        }

        return new VerifyPaymentResponse(
            true,
            false,
            attempt.Order.Id,
            attempt.Order.PaymentStatus.ToString(),
            attempt.RefId ?? verifyResult.RefId,
            verifyResult.Code == 101 ? "Payment already verified." : "Payment verified successfully.");
    }
}

public sealed record StartPaymentResponse(bool IsSuccess, Guid OrderId, string? Reference, string? PaymentUrl, int Code);
public sealed record VerifyPaymentResponse(bool IsSuccess, bool NotFound, Guid? OrderId, string? Status, long? RefId, string Message);

