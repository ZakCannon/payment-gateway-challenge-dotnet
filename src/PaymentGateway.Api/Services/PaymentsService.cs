using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Enums;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Repositories;

namespace PaymentGateway.Api.Services;

public interface IPaymentsService
{
    PostPaymentResponse? Get(Guid id);

    Task<ProcessPaymentResult> ProcessNew(ProcessPaymentRequest req);
}

public class PaymentsService(
    IPaymentValidationService validationService,
    IBankClient bankClient,
    IPaymentsRepository paymentsRepository) : IPaymentsService
{
    public PostPaymentResponse? Get(Guid id) => paymentsRepository.Get(id);

    public async Task<ProcessPaymentResult> ProcessNew(ProcessPaymentRequest req)
    {
        var validationIssues = validationService.ValidateRequest(req);
        if (validationIssues.Any())
        {
            return new ProcessPaymentResult(Result: null, Issues: validationIssues);
        }

        var authRequest = req.ToBankAuthorisationRequest();
        var authResult = await bankClient.Authorise(authRequest);

        var result = new PostPaymentResponse(
            Id: Guid.NewGuid(),
            // TODO work out what Declined is
            Status: authResult.Authorised ? PaymentStatus.Authorized : PaymentStatus.Rejected,
            CardNumberLastFour: int.Parse(req.CardNumber.Substring(req.CardNumber.Length - 4)),
            ExpiryMonth: req.ExpiryMonth,
            ExpiryYear: req.ExpiryYear,
            Currency: req.Currency,
            Amount: req.Amount);

        paymentsRepository.Add(result);
        
        return new ProcessPaymentResult(Result: result, Issues: null);
    }
}