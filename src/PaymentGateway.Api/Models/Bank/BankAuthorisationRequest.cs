using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.Bank;

public record BankAuthorisationRequest(
    [property: JsonPropertyName("card_number")]
    string CardNumber,
    [property: JsonPropertyName("expiry_date")]
    string ExpiryDate,
    [property: JsonPropertyName("currency")]
    string CurrencyCode,
    [property: JsonPropertyName("amount")]
    int Amount,
    [property: JsonPropertyName("cvv")]
    string Cvv);
