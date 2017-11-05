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
            WebHost.CreateDefaultBuilder(args) // Create default builder for webhost
                .UseStartup<Startup>() // Setup how to listen to web requests
                .Build();
    }
}
