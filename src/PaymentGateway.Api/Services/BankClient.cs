using PaymentGateway.Api.Models.Bank;

namespace PaymentGateway.Api.Services;

// Currently a single post to a single endpoint - but I'd expect in a larger system this to
// require other functionality
public interface IBankClient
{
    Task<BankAuthorisationResult> Authorise(BankAuthorisationRequest req);
}

public class BankClient : IBankClient
{
    // Could probably use some logging?
    public async Task<BankAuthorisationResult> Authorise(BankAuthorisationRequest req)
    {
        // TODO Use a HTTP client factory somehow - google it!
        await Task.CompletedTask;
        return new BankAuthorisationResult(true, Guid.NewGuid());
    }
}