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
        // Arrange
        var toAdd = StaticTestData.BlankPostResponse;
        var repo = new PaymentsRepository();
        
        // Act
        repo.Add(toAdd);
        var result = repo.Get(toAdd.Id);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().Be(toAdd);
    }

    [Test]
    public void GivenNoRequestsStored_Get_DoesNotThrow()
    {
        // Arrange
        var repo = new PaymentsRepository();

        // Act
        var result = repo.Get(Guid.Empty);

        // Assert
        result.Should().BeNull();
    }
}