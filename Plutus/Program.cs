using Plutus;
using Plutus.Application;
using Plutus.Application.Gateway;
using Plutus.Services;

var builder = WebApplication.CreateBuilder(args);

var zarinpalOptions = builder.Configuration
    .GetSection(ZarinpalOptions.SectionName)
    .Get<ZarinpalOptions>() ?? new ZarinpalOptions();

builder.Services.AddSingleton(zarinpalOptions);
builder.Services.AddSingleton<IPaymentAttemptStore, InMemoryPaymentAttemptStore>();
builder.Services.AddHttpClient<ZarinPalSandboxGateway>();
builder.Services.AddPaymentGateways(typeof(IPaymentGateway).Assembly);
builder.Services.AddScoped<StartPaymentHandler>();
builder.Services.AddScoped<VerifyPaymentHandler>();
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();