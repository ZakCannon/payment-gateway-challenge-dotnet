using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests.Services;

// Hard to annotate these with arrange/act/assert. The arrange is in the test case, act is the call, and assert is in the return
[TestFixture]
public class PaymentValidationServiceTests
{
    [Test]
    [TestCaseSource(nameof(s_simpleValidationTestCases))]
    public List<ValidationIssue> GivenPaymentRequest_Validate_ReturnsExpectedIssues(ProcessPaymentRequest request)
    {
        var validationService = new PaymentValidationService();

        return validationService.ValidateRequest(request);
    }

    [Test]
    [TestCaseSource(nameof(s_expiryValidationTestCases))]
    public List<ValidationIssue> GivenPaymentRequestWithExpiryDateIssues_Validate_ReturnsExpectedIssues(
        ProcessPaymentRequest request)
    {
        var validationService = new PaymentValidationService();

        return validationService.ValidateRequest(request);
    }

    private static List<TestCaseData> s_simpleValidationTestCases =
    [
        new TestCaseData(ValidRequest)
            .Returns(new List<ValidationIssue>())
            .SetName("Entirely valid should return no issues"),
        new TestCaseData(ValidRequest with { CardNumber = "" })
            .Returns(new List<ValidationIssue>
            {
                new(
                    nameof(ProcessPaymentRequest.CardNumber),
                    "Must be between 14-19 characters")
            })
            .SetName("Card number length should be validated"),
        new TestCaseData(ValidRequest with { Currency = "ZZZZZZ" })
            .Returns(new List<ValidationIssue>
            {
                new(
                    nameof(ProcessPaymentRequest.Currency),
                    "Currency Code is not accepted")
            })
            // Could make accepted currency codes a public list on the PaymentValidationService (or some other configurable value)
            // To make this test more consistent. ZZZZZZ is definitely never valid though, so doesn't matter
            .SetName("Currency code must be valid"),
        new TestCaseData(ValidRequest with { Cvv = "123123123abc" })
            .Returns(new List<ValidationIssue>
            {
                new(
                    nameof(ProcessPaymentRequest.Cvv),
                    "Must be between 3-4 characters"),
                new(
                    nameof(ProcessPaymentRequest.Cvv),
                    "Must only contain numeric characters")
            })
            .SetName("Single field (CVV) can have multiple issues"),
        new TestCaseData(ValidRequest with { Cvv = "1234123", Currency = "ZZZZZ" })
            .Returns(new List<ValidationIssue>
            {
                new(
                    nameof(ProcessPaymentRequest.Currency),
                    "Currency Code is not accepted"),
                new(
                    nameof(ProcessPaymentRequest.Cvv),
                    "Must be between 3-4 characters"),
            })
            .SetName("Multiple fields are validated"),
    ];

    private static List<TestCaseData> s_expiryValidationTestCases =
    [
        new TestCaseData(ValidRequest with { ExpiryMonth = 14 })
            .Returns(new List<ValidationIssue>
            {
                new(
                    nameof(ProcessPaymentRequest.ExpiryMonth),
                    "Expiry month is not a valid month")
            })
            .SetName("Expiry month must be 1-12"),
        new TestCaseData(ValidRequest with { ExpiryMonth = 1, ExpiryYear = 1900 })
            .Returns(new List<ValidationIssue>
            {
                new(
                    $"{nameof(ProcessPaymentRequest.ExpiryMonth)}/{nameof(ProcessPaymentRequest.ExpiryYear)}",
                    "Card has expired")
            })
            .SetName("Expiry must be in the future"),
    ];

    private static ProcessPaymentRequest ValidRequest
        => new ProcessPaymentRequest(
            CardNumber: "12345678912345678",
            ExpiryMonth: 1,
            ExpiryYear: 2100,
            Currency: "USD",
            Amount: 100,
            Cvv: "123");
}