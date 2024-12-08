using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.Bank;

public record BankAuthorisationResult(
    bool Authorised,
    [property: JsonPropertyName("authorisation_code")]
    Guid AuthorisationCode);
