using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Plutus.Application.Gateway;

public class ZarinPalSandboxGateway : IPaymentGateway
{
    private readonly HttpClient _httpClient;
    private readonly ZarinpalOptions _options;

    public ZarinPalSandboxGateway(HttpClient httpClient, ZarinpalOptions options)
    {
        _httpClient = httpClient;
        _options = options;
    }

    public static GatewayType GatewayType => GatewayType.ZarinPalSandbox;

    public async Task<PaymentRequestResult> RequestPaymentAsync(
        long amount,
        string currency,
        string description,
        CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            merchant_id = _options.MerchantId,
            amount,
            currency,
            description,
            callback_url = _options.CallbackUrl
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"{_options.BaseUrl.TrimEnd('/')}/pg/v4/payment/request.json",
            payload,
            cancellationToken);

        var body = await response.Content.ReadFromJsonAsync<ZarinpalApiResponse<ZarinpalRequestData>>(cancellationToken);
        var data = body?.Data;

        if (data?.Code == 100 && !string.IsNullOrWhiteSpace(data.Authority))
        {
            var paymentUrl = $"{_options.BaseUrl.TrimEnd('/')}/pg/StartPay/{data.Authority}";
            return new PaymentRequestResult(true, data.Code, data.Authority, paymentUrl);
        }

        return new PaymentRequestResult(false, data?.Code ?? 0, null, null);
    }

    public async Task<PaymentVerifyResult> VerifyPaymentAsync(
        long amount,
        string reference,
        CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            merchant_id = _options.MerchantId,
            amount,
            authority = reference
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"{_options.BaseUrl.TrimEnd('/')}/pg/v4/payment/verify.json",
            payload,
            cancellationToken);

        var body = await response.Content.ReadFromJsonAsync<ZarinpalApiResponse<ZarinpalVerifyData>>(cancellationToken);
        var data = body?.Data;

        return new PaymentVerifyResult(
            data?.Code is 100 or 101,
            data?.Code ?? 0,
            data?.RefId);
    }

    private sealed class ZarinpalApiResponse<T>
    {
        [JsonPropertyName("data")]
        public T? Data { get; set; }
    }

    private sealed class ZarinpalRequestData
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("authority")]
        public string? Authority { get; set; }
    }

    private sealed class ZarinpalVerifyData
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("ref_id")]
        public long? RefId { get; set; }
    }
}