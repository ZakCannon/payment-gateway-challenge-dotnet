namespace PaymentGateway.Api.Models.Requests;

public record PostPaymentRequest(
    int CardNumberLastFour,
    int ExpiryMonth,
    int ExpiryYear,
    string Currency,
    int Amount,
    int Cvv);