using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Library.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;

namespace Common.Library.HealthChecks;
public static class Extentions
{
    private const string MongoCheckName = "mongodb";
    private const string HealthEndpoint = "health";
    private const string ReadyTagName = "ready";
    private const string LiveTagName = "live";
    private const int DefaultSeconds = 3;
    public static IHealthChecksBuilder AddMongoDbHealthCheck(this IHealthChecksBuilder builder, TimeSpan?
        timeout = default)
    {
        return builder.Add(new HealthCheckRegistration(
            MongoCheckName,
            serviceProvider =>
            {
                var configuration = serviceProvider.GetService<IConfiguration>();
                var mongoDbSettings =
                    configuration!.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
                var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
                return new MongoDbHealthCheck(mongoClient);
            },
            HealthStatus.Unhealthy,
            new[] { ReadyTagName },
            timeout ?? TimeSpan.FromSeconds(DefaultSeconds)));
    }
    public static void MapCustomHealthChecks(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks($"/{HealthEndpoint}/{ReadyTagName}", new HealthCheckOptions()
        {
            Predicate = (check) => check.Tags.Contains(ReadyTagName),
        });
        endpoints.MapHealthChecks($"/{HealthEndpoint}/{LiveTagName}", new HealthCheckOptions()
        {
            Predicate = (_) => false
        });
    }

}
