namespace PaymentGateway.Api.Models;

public record ValidationIssue(
    string FieldName,
    string Message);