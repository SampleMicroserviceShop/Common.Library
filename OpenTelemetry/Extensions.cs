using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Library.MassTransit;
using Common.Library.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Common.Library.OpenTelemetry;
public static class Extensions
{
    public static IServiceCollection AddTracing(this IServiceCollection services, IConfiguration config)
    {
        var serviceSettings = config.GetSection(nameof(ServiceSettings))
            .Get<ServiceSettings>();

        services.AddSingleton<ConsumeObserver>();
        services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {

                tracerProviderBuilder
                    .AddSource(serviceSettings.ServiceName)
                    .AddSource("MassTransit")
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(serviceSettings.ServiceName))
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddConsoleExporter()
                    .AddOtlpExporter(options =>
                    {
                        var jaegerSettings = config.GetSection(nameof(JaegerSettings))
                            .Get<JaegerSettings>();
                        options.Endpoint = new Uri($"http://{jaegerSettings.Host}:{jaegerSettings.Port}"); // gRPC

                    });
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddMeter(serviceSettings.ServiceName)
                    .AddMeter("MassTransit")
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddPrometheusExporter();
            });
        return services;
    }
}
