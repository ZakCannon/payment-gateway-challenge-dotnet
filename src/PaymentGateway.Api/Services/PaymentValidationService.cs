using PaymentGateway.Api.Extensions;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Services;

public interface IPaymentValidationService
{
    List<ValidationIssue> ValidateRequest(ProcessPaymentRequest request);
}

public class PaymentValidationService : IPaymentValidationService
{
    public List<ValidationIssue> ValidateRequest(ProcessPaymentRequest request)
    {
        var cardNumberIssues = ValidateCardNumber(request.CardNumber);
        var expirationDateIssues = ValidateExpirationDate(request.ExpiryMonth, request.ExpiryYear);
        var currencyCodeIssues = ValidateCurrencyCode(request.Currency);
        var cvvIssues = ValidateCvv(request.Cvv);

        return cardNumberIssues
            .Concat(expirationDateIssues)
            .Concat(currencyCodeIssues)
            .Concat(cvvIssues)
            .ToList();
    }

    private List<ValidationIssue> ValidateCardNumber(string cardNumber)
        => ValidateNumericStringOfLength(cardNumber, nameof(ProcessPaymentRequest.CardNumber), 14, 19);

    private List<ValidationIssue> ValidateCvv(string cvv)
        => ValidateNumericStringOfLength(cvv, nameof(ProcessPaymentRequest.Cvv), 3, 4);

    private List<ValidationIssue> ValidateExpirationDate(int expiryMonth, int expiryYear)
    {
        var expiryMonthIsNotValid = expiryMonth is > 12 or < 1;
        if (expiryMonthIsNotValid)
        {
            return
            [
                new ValidationIssue(
                    FieldName: nameof(ProcessPaymentRequest.ExpiryMonth),
                    Message: "Expiry month is not a valid month")
            ];
        }

        var currentDate = DateOnly.FromDateTime(DateTime.Now);
        // Following logic will consider an expiry date in the current month as out of date.
        // Easy to change to allow by setting month to expiryMonth + 1 on below line.
        var expiryDate = new DateOnly(expiryYear, expiryMonth, 1);
        if (expiryDate < currentDate)
        {
            return
            [
                new ValidationIssue(
                    FieldName:
                    $"{nameof(ProcessPaymentRequest.ExpiryMonth)}/{nameof(ProcessPaymentRequest.ExpiryYear)}",
                    Message: "Card has expired")
            ];
        }

        return [];
    }

    // Could be expanded to any currency by checking all caps and 3 letters long
    private List<ValidationIssue> ValidateCurrencyCode(string currencyCode)
    {
        List<string> acceptedCurrencyCodes = ["GBP", "EUR", "USD"];
        return acceptedCurrencyCodes.Contains(currencyCode)
            ? []
            :
            [
                new ValidationIssue(
                    FieldName: nameof(ProcessPaymentRequest.Currency),
                    Message: "Currency Code is not accepted")
            ];
    }

    private List<ValidationIssue> ValidateNumericStringOfLength(
        string value,
        string fieldName,
        int minLength,
        int maxLength)
    {
        List<ValidationIssue> issues = [];
        
        var isWrongLength = value.Length < minLength || value.Length > maxLength;
        var containsNonNumericCharacters = !value.IsNumeric();

        if (isWrongLength)
        {
            issues.Add(new ValidationIssue(
                FieldName: fieldName,
                Message: $"Must be between {minLength}-{maxLength} characters"));
        }

        if (containsNonNumericCharacters)
        {
            issues.Add(new ValidationIssue(
                FieldName: fieldName,
                Message: "Must only contain numeric characters"));
        }

        return issues;
    }
}