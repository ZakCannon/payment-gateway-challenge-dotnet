using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Repositories;

public interface IPaymentsRepository
{
    void Add(PostPaymentResponse payment);

    PostPaymentResponse? Get(Guid id);
}

// Here I'd normally use a DbContext from EF core - list will do for now
// Maybe look at how hard this is to add
public class PaymentsRepository : IPaymentsRepository
{
    // this should probably be a different model - eg PaymentEntity - not the response object
    private readonly List<PostPaymentResponse> _payments = [];
    
    public void Add(PostPaymentResponse payment) => _payments.Add(payment);

    public PostPaymentResponse? Get(Guid id) => _payments.FirstOrDefault(p => p.Id == id);
}