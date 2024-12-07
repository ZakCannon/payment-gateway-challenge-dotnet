using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Repository;

public interface IPaymentsRepository
{
    void Add(PostPaymentResponse payment);

    PostPaymentResponse? Get(Guid id);
}

// Here I'd normally use a DbContext from EF core - list will do for now
// Maybe look at how hard this is to add
public class PaymentsRepository : IPaymentsRepository
{
    // TODO this should be a different model - not the response!
    private readonly List<PostPaymentResponse> _payments = [];
    
    public void Add(PostPaymentResponse payment)
    {
        _payments.Add(payment);
    }

    public PostPaymentResponse? Get(Guid id)
    {
        return _payments.FirstOrDefault(p => p.Id == id);
    }
}