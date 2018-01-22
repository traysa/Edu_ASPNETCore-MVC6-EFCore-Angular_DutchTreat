using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DutchTreat.Data
{
    /// <summary>
    /// Also read: https://wildermuth.com/2018/01/10/Re-thinking-Running-Migrations-and-Seeding-in-ASP-NET-Core-2-0
    /// </summary>
    public class DutchContextFactory : IDesignTimeDbContextFactory<DutchContext>
    {
        public DutchContext CreateDbContext(string[] args)
        {
            // Create a configuration 
            var builder = new WebHostBuilder();
            var config = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("config.json")
              .Build();

            return new DutchContext(new DbContextOptionsBuilder<DutchContext>().Options, config);
        }
    }
}