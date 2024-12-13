using PaymentGateway.Api.Models.Enums;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Models.Responses;

public record PostPaymentResponse(
    Guid Id,
    PaymentStatus Status,
    int CardNumberLastFour,
    int ExpiryMonth,
    int ExpiryYear,
    string Currency,
    int Amount)
{
    public static PostPaymentResponse FromPaymentRequest(ProcessPaymentRequest req, PaymentStatus status) => new(
        Id: Guid.NewGuid(),
        Status: status,
        CardNumberLastFour: int.Parse(req.CardNumber.Substring(req.CardNumber.Length - 4)),
        ExpiryMonth: req.ExpiryMonth,
        ExpiryYear: req.ExpiryYear,
        Currency: req.Currency,
        Amount: req.Amount);
}