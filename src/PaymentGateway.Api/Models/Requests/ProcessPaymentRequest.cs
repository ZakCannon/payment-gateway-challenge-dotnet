using PaymentGateway.Api.Models.Bank;

namespace PaymentGateway.Api.Models.Requests;

// Fields straight out of README
public record ProcessPaymentRequest(
    string CardNumber,
    int ExpiryMonth,
    int ExpiryYear,
    string Currency,
    int Amount,
    string Cvv)
{
    public BankAuthorisationRequest ToBankAuthorisationRequest()
        => new(
            CardNumber: CardNumber,
            ExpiryDate: $"{ExpiryMonth}/{ExpiryYear}",
            CurrencyCode: Currency,
            Amount: Amount,
            Cvv: Cvv);
}
