namespace PaymentGateway.Api.Models.Bank;

public record BankAuthorisationResult(
    bool Authorised,
    Guid AuthorisationCode);
