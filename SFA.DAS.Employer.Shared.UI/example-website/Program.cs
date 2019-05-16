﻿using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;

namespace DfE.Example.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureKestrel(c => c.AddServerHeader = false)
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var environmentName = hostingContext.HostingEnvironment.EnvironmentName;
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appSettings.json", optional: false, reloadOnChange: false);
                    config.AddAzureTableStorage(options => {
                        options.ConfigurationKeys = new [] { "SFA.DAS.Employer.Shared.UI" };
                        options.EnvironmentNameEnvironmentVariableName = "APPSETTING_EnvironmentName";
                        options.StorageConnectionStringEnvironmentVariableName = "APPSETTING_ConfigurationStorageConnectionString";
                    });
                    config.AddUserSecrets<Startup>();
                    config.AddEnvironmentVariables();
                })
                .UseUrls("https://localhost:5040");
    }
}
