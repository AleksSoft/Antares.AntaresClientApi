﻿using System;
using System.Collections.Generic;
using AntaresClientApi.Database.MeData;
using Lykke.Common.Log;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Events;
using Swisschain.Sdk.Server.Common;
using Swisschain.Sdk.Server.Logging;

namespace AntaresClientApi
{
    public class Program
    {
        private sealed class RemoteSettingsConfig
        {
            public IReadOnlyCollection<string> RemoteSettingsUrls { get; set; }
        }

        
        public static void Main(string[] args)
        {
            Console.Title = "Antares AntaresClientApi";

            var remoteSettingsConfig = ApplicationEnvironment.Config.Get<RemoteSettingsConfig>();

            var loggerFactory = LogConfigurator.Configure("Antares", remoteSettingsConfig.RemoteSettingsUrls ?? Array.Empty<string>());

            MeWriterDataContext.LoggerFactory = loggerFactory;

            var logger = loggerFactory.CreateLogger<Program>();

            try
            {
                logger.LogInformation("Application is being started");

                CreateHostBuilder(loggerFactory, remoteSettingsConfig).Build().Run();

                logger.LogInformation("Application has been stopped");
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Application has been terminated unexpectedly");
            }
        }

        private static IHostBuilder CreateHostBuilder(ILoggerFactory loggerFactory, RemoteSettingsConfig remoteSettingsConfig) =>
            new HostBuilder()
                .SwisschainService<Startup>(options =>
                {
                    options.UseLoggerFactory(loggerFactory);
                    options.AddWebJsonConfigurationSources(remoteSettingsConfig.RemoteSettingsUrls ?? Array.Empty<string>());
                });
    }
}
