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
            var failedResult = PostPaymentResponse.FromPaymentRequest(req, PaymentStatus.Rejected);
            paymentsRepository.Add(failedResult);
            return new ProcessPaymentResult(Result: failedResult, Issues: validationIssues);
        }

        var authRequest = req.ToBankAuthorisationRequest();
        var authResult = await bankClient.Authorise(authRequest);

        var status = authResult.Authorised ? PaymentStatus.Authorized : PaymentStatus.Declined;
        var successfulResult = PostPaymentResponse.FromPaymentRequest(req, status);

        paymentsRepository.Add(successfulResult);
        
        return new ProcessPaymentResult(Result: successfulResult, Issues: null);
    }
}