using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Common.Library.Settings;
using Microsoft.Extensions.Logging;

namespace Common.Library.Logging;
public static class Extensions
{
    public static IServiceCollection AddSeqLogging(this IServiceCollection services, IConfiguration config)
    {
        services.AddLogging(loggingBuilder =>
        {
            var seqSettings = config.GetSection(nameof(SeqSettings)).Get<SeqSettings>();
            loggingBuilder.AddSeq(serverUrl: seqSettings.ServerUrl);
        });
        return services;
    }
}
