using PaymentGateway.Api.Models.Enums;

namespace PaymentGateway.Api.Models.Responses;

public record PostPaymentResponse(
    Guid Id,
    PaymentStatus Status,
    int CardNumberLastFour,
    int ExpiryMonth,
    int ExpiryYear,
    string Currency,
    int Amount);