using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(
    IPaymentsService paymentsService) : Controller
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        var payment = paymentsService.Get(id);

        return new OkObjectResult(payment);
    }

    [HttpPost("")]
    public async Task<ActionResult<PostPaymentResponse>> PostPaymentAsync([FromBody] ProcessPaymentRequest req)
    {
        var result = await paymentsService.ProcessNew(req);
        if (result.Issues is not null)
        {
            return new BadRequestObjectResult(result.Issues);
        }

        return new OkObjectResult(result.Result);
    }
}