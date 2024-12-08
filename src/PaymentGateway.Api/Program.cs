using PaymentGateway.Api.Config;
using PaymentGateway.Api.Repositories;
using PaymentGateway.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.Development.json", optional: false);
var config = new PaymentServiceConfig();
builder.Configuration.Bind(config);
builder.Services.AddSingleton(config);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddTransient<IBankClient, BankClient>();
builder.Services.AddTransient<IPaymentsService, PaymentsService>();
builder.Services.AddTransient<IPaymentValidationService, PaymentValidationService>();
// Would use transient repository and a real DbContext in a normal app
// Require singleton to track state in the list
builder.Services.AddSingleton<IPaymentsRepository, PaymentsRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
