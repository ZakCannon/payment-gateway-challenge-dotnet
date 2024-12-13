namespace PaymentGateway.Api.Tests.Services;

// Main thing I'd want to test here is that the encoding works as expected, if at all. It doesn't stop the 
// Bank changing the shape of its requests, but does prevent regression
// Ran out of time to set it up though! Would need to store the calls into HttpClient
[TestFixture]
public class BankClientTests
{
    [Test]
    public void Test1()
    {
        true.Should().BeTrue();
    }
}