using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Plutus.Application.Gateway;

namespace Plutus;

public static class GatewayRegistrationExtensions
{
    public static IServiceCollection AddPaymentGateways(this IServiceCollection services, Assembly assembly)
    {
        var gatewayTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && t.IsAssignableTo(typeof(IPaymentGateway)));

        foreach (var implementationType in gatewayTypes)
        {
            var property = implementationType.GetProperty(
                nameof(IPaymentGateway.GatewayType),
                BindingFlags.Public | BindingFlags.Static);

            var gatewayKey = (GatewayType)property!.GetValue(null)!;

            services.AddKeyedScoped(typeof(IPaymentGateway), gatewayKey, implementationType);
        }

        services.AddScoped<IPaymentGatewayFactory, PaymentGatewayFactory>();
        return services;
    }
}

