using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using PaymentGateway.Api.Config;
using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Models.Enums;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Repositories;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests;

[TestFixture]
public class PaymentsControllerTests
{
    private readonly Random _random = new();
    private readonly Mock<IBankClient> _bankClient = new();
    
    [Test]
    public async Task RetrievesAPaymentSuccessfully()
    {
        // Arrange
        var payment = new PostPaymentResponse(
            Id: Guid.NewGuid(),
            Status: PaymentStatus.Authorized,
            ExpiryYear: _random.Next(2023, 2030),
            ExpiryMonth: _random.Next(1, 12),
            Amount: _random.Next(1, 10000),
            CardNumberLastFour: _random.Next(1111, 9999),
            Currency: "GBP");

        var paymentsRepository = new PaymentsRepository();
        paymentsRepository.Add(payment);
        var client = BuildClient(paymentsRepository);

        // Act
        var response = await client.GetAsync($"/api/Payments/{payment.Id}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        paymentResponse.Should().NotBeNull();
    }

    [Test]
    public async Task Returns404IfPaymentNotFound()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();
        
        // Act
        var response = await client.GetAsync($"/api/Payments/{Guid.NewGuid()}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static HttpClient BuildClient(PaymentsRepository paymentsRepository)
    {
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        return webApplicationFactory.WithWebHostBuilder(builder =>
            builder
                .ConfigureServices(services => ((ServiceCollection)services)
                .AddSingleton<IPaymentsRepository>(paymentsRepository)
                .AddTransient<IBankClient, BankClient>()
                .AddTransient<IPaymentsService, PaymentsService>()
                .AddTransient<IPaymentValidationService, PaymentValidationService>())
            )
            .CreateClient();
    }
}