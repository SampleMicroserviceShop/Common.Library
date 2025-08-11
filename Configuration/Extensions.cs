using Azure.Identity;
using Common.Library.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Common.Library.Configuration;
public static class Extensions
{
    public static void ConfigureAzureKeyVault(this ConfigurationManager configuration, IWebHostEnvironment env)
    {
        if (env.IsProduction())
        {
            var serviceSettings =
                configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
            configuration.AddAzureKeyVault(
                new Uri($"https://{serviceSettings.KeyVaultName}.vault.azure.net/"),
                new DefaultAzureCredential());
        }
    }
}
