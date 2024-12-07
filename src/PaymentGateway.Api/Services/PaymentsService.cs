using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Repository;

namespace PaymentGateway.Api.Services;

public interface IPaymentsService
{
    PostPaymentResponse? Get(Guid Id);
}

public class PaymentsService(
    IPaymentsRepository paymentsRepository) : IPaymentsService
{
    public PostPaymentResponse? Get(Guid Id) => paymentsRepository.Get(Id);
}