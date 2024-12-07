namespace PaymentGateway.Api.Models.Bank;

public record BankAuthorisationRequest(
    string CardNumber,
    string ExpiryDate,
    string CurrencyCode,
    int Amount,
    string Cvv);
