using Moq;

using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Bank;
using PaymentGateway.Api.Models.Enums;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Repositories;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests.Services;

[TestFixture]
public class PaymentServiceTests
{
    private PaymentsService _paymentsService;
    private readonly Mock<IPaymentValidationService> _validationService = new();
    private readonly Mock<IBankClient> _bankClient = new();
    private readonly Mock<IPaymentsRepository> _paymentsRepository = new();

    [SetUp]
    public void SetUp()
    {
        _validationService.Reset();
        _bankClient.Reset();
        _paymentsRepository.Reset();
        _paymentsService = new PaymentsService(
            _validationService.Object,
            _bankClient.Object,
            _paymentsRepository.Object);
    }

    [Test]
    public void GivenValidId_Get_ShouldReturnPayment()
    {
        var idToFetch = Guid.NewGuid();
        _paymentsRepository
            .Setup(r => r.Get(idToFetch))
            .Returns(StaticTestData.BlankPostResponse);

        var result = _paymentsService.Get(idToFetch);

        result.Should().Be(StaticTestData.BlankPostResponse);
    }

    [Test]
    public async Task GivenValidAuthorisedRequest_ProcessNew_ShouldAddToRepositoryAndReturn()
    {
        var requestToAdd = BasicRequest;
        _validationService
            .Setup(s => s.ValidateRequest(requestToAdd))
            .Returns([]);
        _bankClient
            .Setup(c => c.Authorise(requestToAdd.ToBankAuthorisationRequest()))
            .ReturnsAsync(new BankAuthorisationResult(true, Guid.NewGuid()));

        var result = await _paymentsService.ProcessNew(requestToAdd);

        result.Result.Should().NotBeNull();
        VerifyRequestResponseBasicFieldsMatch(requestToAdd, result.Result);
        result.Result.Status.Should().Be(PaymentStatus.Authorized);
        result.Issues.Should().BeNull();

        _paymentsRepository
            .Verify(r => r.Add(
                It.Is<PostPaymentResponse>(res => res.Id == result.Result.Id)));
    }

    [Test]
    public async Task GivenFailingValidation_ProcessNew_ShouldNotContactBankOrRepositoryAndReturnIssues()
    {
        var blankValidationIssue = new ValidationIssue("", "");
        _validationService
            .Setup(s => s.ValidateRequest(BasicRequest))
            .Returns([blankValidationIssue]);

        var result = await _paymentsService.ProcessNew(BasicRequest);

        result.Result.Should().BeNull();
        result.Issues.Should().NotBeNull();
        result.Issues.Count.Should().Be(1);
        result.Issues.Single().Should().Be(blankValidationIssue);

        _bankClient.VerifyNoOtherCalls();
        _paymentsRepository.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GivenBankRejecting_ProcessNew_ShouldReturnRejectedPayment()
    {
        var requestToAdd = BasicRequest;
        _validationService
            .Setup(s => s.ValidateRequest(requestToAdd))
            .Returns([]);
        _bankClient
            .Setup(c => c.Authorise(requestToAdd.ToBankAuthorisationRequest()))
            .ReturnsAsync(new BankAuthorisationResult(false, Guid.NewGuid()));

        var result = await _paymentsService.ProcessNew(requestToAdd);

        result.Result.Should().NotBeNull();
        VerifyRequestResponseBasicFieldsMatch(requestToAdd, result.Result);
        result.Result.Status.Should().Be(PaymentStatus.Rejected);
        result.Issues.Should().BeNull();

        _paymentsRepository
            .Verify(r => r.Add(
                It.Is<PostPaymentResponse>(res => res.Id == result.Result.Id)));
    }

    private static void VerifyRequestResponseBasicFieldsMatch(ProcessPaymentRequest req, PostPaymentResponse res)
    {
        res.ExpiryYear.Should().Be(req.ExpiryYear);
        res.ExpiryMonth.Should().Be(req.ExpiryMonth);
        res.Currency.Should().Be(req.Currency);
        res.Amount.Should().Be(req.Amount);
        req.CardNumber.EndsWith(res.CardNumberLastFour.ToString()).Should().BeTrue();
    }

    private static ProcessPaymentRequest BasicRequest
        => new(
            CardNumber: "1234123412341234",
            ExpiryMonth: 1,
            ExpiryYear: 2024,
            Currency: "USD",
            Amount: 1,
            Cvv: "123");
}