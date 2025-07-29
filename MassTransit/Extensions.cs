using System.Reflection;
using Common.Library.Settings;
using GreenPipes;
using GreenPipes.Configurators;
using MassTransit;
using MassTransit.Definition;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Library.MassTransit;
public static class Extensions
{
    public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services,
        Action<IRetryConfigurator> configureRetries = null)
    {
        services.AddMassTransit(configure =>
        {
            configure.AddConsumers(Assembly.GetEntryAssembly());
            configure.UsingCustomRabbitMQ(configureRetries);
        });

        services.AddMassTransitHostedService();
        return services;
    }
    public static void UsingCustomRabbitMQ(
        this IServiceCollectionBusConfigurator configure,
        Action<IRetryConfigurator> configureRetries = null)
    {
        configure.UsingRabbitMq((context, configurator) =>
        {
            var configuration = context.GetService<IConfiguration>();
            var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
            var rabbitMQSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
            configurator.Host(rabbitMQSettings.Host);
            configurator.ConfigureEndpoints(
                context,
                new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
            if (configureRetries == null)
            {
                configureRetries = (retryConfigurator) => retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
            }
            configurator.UseMessageRetry(configureRetries);
        });
    }

}
