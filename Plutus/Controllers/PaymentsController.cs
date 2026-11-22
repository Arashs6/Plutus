using Microsoft.AspNetCore.Mvc;
using Plutus.Application;
using Plutus.Application.Gateway;

namespace Plutus.Controllers;

[ApiController]
[Route("payments/{gatewayType}")]
public class PaymentsController : ControllerBase
{
    private readonly StartPaymentHandler _startPaymentHandler;
    private readonly VerifyPaymentHandler _verifyPaymentHandler;

    public PaymentsController(
        StartPaymentHandler startPaymentHandler,
        VerifyPaymentHandler verifyPaymentHandler)
    {
        _startPaymentHandler = startPaymentHandler;
        _verifyPaymentHandler = verifyPaymentHandler;
    }

    [HttpPost("start")]
    public async Task<IActionResult> Start(GatewayType gatewayType, CancellationToken cancellationToken)
    {
        var result = await _startPaymentHandler.HandleAsync(gatewayType, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = "Payment request failed.", code = result.Code });
        }

        return Ok(new
        {
            orderId = result.OrderId,
            reference = result.Reference,
            paymentUrl = result.PaymentUrl
        });
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback(
        GatewayType gatewayType,
        [FromQuery(Name = "Authority")] string? authority,
        [FromQuery(Name = "Status")] string? status,
        CancellationToken cancellationToken)
    {
        var result = await _verifyPaymentHandler.HandleAsync(gatewayType, authority, status, cancellationToken);
        if (result.NotFound)
        {
            return NotFound(new { message = result.Message });
        }

        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                message = result.Message,
                orderId = result.OrderId,
                status = result.Status
            });
        }

        return Ok(new
        {
            message = result.Message,
            orderId = result.OrderId,
            status = result.Status,
            refId = result.RefId
        });
    }
}

