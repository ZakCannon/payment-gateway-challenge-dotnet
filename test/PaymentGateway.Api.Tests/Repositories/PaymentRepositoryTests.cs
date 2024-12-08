using PaymentGateway.Api.Models.Enums;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Repositories;

namespace PaymentGateway.Api.Tests.Repositories;

[TestFixture]
public class PaymentRepositoryTests
{
    [Test]
    public void GivenRequestStored_Get_CanRetrieveRequest()
    {
        var toAdd = StaticTestData.BlankPostResponse;

        var repo = new PaymentsRepository();
        repo.Add(toAdd);

        var result = repo.Get(toAdd.Id);
        
        result.Should().NotBeNull();
        result.Should().Be(toAdd);
    }

    [Test]
    public void GivenNoRequestsStored_Get_DoesNotThrow()
    {
        var repo = new PaymentsRepository();

        var result = repo.Get(Guid.Empty);

        result.Should().BeNull();
    }
}