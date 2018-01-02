using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DutchTreat
{
    public class Program
    {
        /// <summary>
        /// Console app starting the webhost
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            // Building a webhost and start listening to requests
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args) // Create default builder for webhost; Creates also a default configuration file
                .ConfigureAppConfiguration(SetupConfiguration) // Configuration builder to add own configuration options
                .UseStartup<Startup>() // Setup how to listen to web requests
                .Build();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="builder"></param>
        private static void SetupConfiguration(WebHostBuilderContext ctx, IConfigurationBuilder builder)
        {
            // Removing the default configuration options
            builder.Sources.Clear();

            // Define configuration files (Different to .NET)
            // All configuration are loaded into one store; If there is a conflict the latest 
            // configuration wins, e.g. the environment variables would overwrite JSON properties
            builder.AddJsonFile("config.json", false, true) // mandatory and reload on change
                   .AddXmlFile("config.xml", true) // optional
                   .AddEnvironmentVariables();
        }
    }
}
