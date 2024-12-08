using PaymentGateway.Api.Models.Enums;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Tests;

public static class StaticTestData
{
    public static PostPaymentResponse BlankPostResponse => new(
        Id: Guid.Empty,
        Status: PaymentStatus.Authorized,
        CardNumberLastFour: 1234,
        ExpiryMonth: 1,
        ExpiryYear: 1,
        Currency: "",
        Amount: 0);
}