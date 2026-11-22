namespace Plutus.Application.Gateway;

public sealed class ZarinpalOptions
{
    public const string SectionName = "Zarinpal";

    public string BaseUrl { get; set; } = "https://sandbox.zarinpal.com";
    public string MerchantId { get; set; } = string.Empty;
    public string CallbackUrl { get; set; } = string.Empty;
}

