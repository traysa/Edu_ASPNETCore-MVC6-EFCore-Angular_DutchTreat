using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DutchTreat.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DutchTreat.Data
{
    /// <summary>
    /// Interface to the data store
    /// DbContext represents a class, which knows how to execute querries to a data store
    /// </summary>
    public class DutchContext : IdentityDbContext<StoreUser>// Define the IdentityUser type "StoreUser" as Generic argument (by default it would be Identity User without the extra properties)
    {
        private IConfiguration _config;

        /// <summary>
        /// Constructor calls base class contructor and passes on the options.
        /// When registering the db context in Startup.cs the options contain the necessary information about db provider and connection string.
        /// </summary>
        /// <param name="options"></param>
        public DutchContext(DbContextOptions<DutchContext> options, IConfiguration config) : base(options)
        {
            _config = config;
        }

        // Override the DB connection 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_config["ConnectionStrings:DutchConnectionString"]);
            base.OnConfiguring(optionsBuilder);
        }

        // The following properties allow to query the entities for getting or adding
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        // A property for OrderItem is not needed, since there is a relation between Order and OrderItem
        
    }
}
