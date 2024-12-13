using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Models;

public record ProcessPaymentResult(
    PostPaymentResponse Result,
    List<ValidationIssue>? Issues);