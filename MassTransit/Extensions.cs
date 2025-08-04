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
    private const string RabbitMq = "RABBITMQ";
    private const string ServiceBus = "SERVICEBUS";
    public static IServiceCollection AddMassTransitWithMessageBroker(
        this IServiceCollection services,
        IConfiguration config,
        Action<IRetryConfigurator> configureRetries = null)
    {
        var serviceSettings = config.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
        switch (serviceSettings.MessageBroker?.ToUpper())
        {
            case ServiceBus:
                services.AddMassTransitWithServiceBus(configureRetries);
                break;
            case RabbitMq:
            default:
                services.AddMassTransitWithRabbitMq(configureRetries);
                break;
        }
        return services;
    }

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
    public static IServiceCollection AddMassTransitWithServiceBus(
        this IServiceCollection services,
        Action<IRetryConfigurator> configureRetries = null)
    {
        services.AddMassTransit(configure =>
        {
            configure.AddConsumers(Assembly.GetEntryAssembly());
            configure.UsingCustomAzureServiceBus(configureRetries);
        });
        services.AddMassTransitHostedService();
        return services;
    }
    public static void UsingCustomMessageBroker(
        this IServiceCollectionBusConfigurator configure,
        IConfiguration config,
        Action<IRetryConfigurator> configureRetries = null)
    {
        var serviceSettings = config.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
        switch (serviceSettings.MessageBroker?.ToUpper())
        {
            case ServiceBus:
                configure.UsingCustomAzureServiceBus(configureRetries);
                break;
            case RabbitMq:
            default:
                configure.UsingCustomRabbitMQ(configureRetries);
                break;
        }
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
    public static void UsingCustomAzureServiceBus(
        this IServiceCollectionBusConfigurator configure,
        Action<IRetryConfigurator> configureRetries = null)
    {
        configure.UsingAzureServiceBus((context, configurator) =>
        {
            var configuration = context.GetService<IConfiguration>();
            var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
            var serviceBusSettings =
                configuration.GetSection(nameof(ServiceBusSettings)).Get<ServiceBusSettings>();
            configurator.Host(serviceBusSettings.ConnectionString);
            configurator.ConfigureEndpoints(context, new
                KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
            if (configureRetries == null)
            {
                configureRetries = (retryConfigurator) => retryConfigurator.Interval(3,
                    TimeSpan.FromSeconds(5));
            }
            configurator.UseMessageRetry(configureRetries);
        });
    }


}
