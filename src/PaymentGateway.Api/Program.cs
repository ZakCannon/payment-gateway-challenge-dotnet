using PaymentGateway.Api.Repository;
using PaymentGateway.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IPaymentsService, PaymentsService>();
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
