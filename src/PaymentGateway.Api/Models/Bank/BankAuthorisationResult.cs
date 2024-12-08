using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.Bank;

public record BankAuthorisationResult(
    [property: JsonPropertyName("authorized")]
    bool Authorised,
    [property: JsonPropertyName("authorization_code")]
    Guid AuthorisationCode);
