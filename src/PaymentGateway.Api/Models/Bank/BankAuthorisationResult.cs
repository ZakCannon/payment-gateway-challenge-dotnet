using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.Bank;

// I wanted to bind to a Guid? type for AuthorisationCode, but "" is not a valid Guid
// I could add some special JSON deserialising - but since we don't actually do anything with it that requires it
// to be a Guid, I don't need to. Could also just add a .IsNullOrEmpty before we Guid.Parse in some code down the line.
public record BankAuthorisationResult(
    [property: JsonPropertyName("authorized")]
    bool Authorised,
    [property: JsonPropertyName("authorization_code")]
    string AuthorisationCode);
