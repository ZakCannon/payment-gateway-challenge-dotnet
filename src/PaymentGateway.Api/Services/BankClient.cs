using System.Net;
using System.Text;
using System.Text.Json;

using PaymentGateway.Api.Config;
using PaymentGateway.Api.Models.Bank;

namespace PaymentGateway.Api.Services;

// Currently a single post to a single endpoint - but I'd expect in a larger system this to
// require other functionality
public interface IBankClient
{
    Task<BankAuthorisationResult> Authorise(BankAuthorisationRequest req);
}

public class BankClient(
    IHttpClientFactory httpClientFactory,
    PaymentServiceConfig config) : IBankClient
{
    public async Task<BankAuthorisationResult> Authorise(BankAuthorisationRequest req)
    {
        var client = httpClientFactory.CreateClient();
        var rawResponse = await client.PostAsync(
            $"{config.BankApiBaseUrl}/payments",
            new StringContent(
                JsonSerializer.Serialize(req),
                Encoding.UTF8, 
                // I'm surprised there's not an Enum for this!
                "application/json"));

        if (rawResponse.StatusCode != HttpStatusCode.OK)
        {
            // In a real service this exception might need sanitising, especially if it gets passed to a user
            throw new Exception("Passed a request which the simulator does not support. Full response was: " + await rawResponse.Content.ReadAsStringAsync());
        }

        var asString = await rawResponse.Content.ReadAsStringAsync();
        var response = JsonSerializer.Deserialize<BankAuthorisationResult>(asString);
        
        return response!;
    }
}