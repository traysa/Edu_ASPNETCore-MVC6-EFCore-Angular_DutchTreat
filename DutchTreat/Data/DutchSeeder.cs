using DutchTreat.Data.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DutchTreat.Data
{
    public class DutchSeeder
    {
        private readonly DutchContext _ctx;
        private readonly IHostingEnvironment _hosting;
        private readonly UserManager<StoreUser> _userManager;

        public DutchSeeder(DutchContext ctx, IHostingEnvironment hosting, UserManager<StoreUser> userManager)
        {
            _ctx = ctx;
            _hosting = hosting;
            _userManager = userManager;
        }

        // Initialize database with some data
        public async Task Seed()
        {
            // Check if database exists
            _ctx.Database.EnsureCreated();

            // Get or Create user
            var user = await _userManager.FindByEmailAsync("admin@dutchtreat.com"); // Since the FindByEmailAsync method is async the current method "Seed" must be also async and return a Task - only then we can also use await in front of FindByEmailAsync

            if (user == null)
            {
                // If user has not been created yet, create it
                user = new StoreUser()
                {
                    FirstName = "traysa",
                    LastName = "admin",
                    UserName = "admin@dutchtreat.com",
                    Email = "admin@dutchtreat.com"
                };

                var result = await _userManager.CreateAsync(user, "P@ssw0rd!"); // returns an identity result to test againast
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Failed to create default user");
                }
            }

            // If there are no products in the database, create some default products
            // and a sample order
            if (!_ctx.Products.Any())
            {
                // Need to create sample data
                var filepath = Path.Combine(_hosting.ContentRootPath, "Data/art.json");
                var json = File.ReadAllText(filepath);
                var products = JsonConvert.DeserializeObject<IEnumerable<Product>>(json);
                _ctx.Products.AddRange(products);
                
                var order = new Order()
                {
                    OrderDate = DateTime.Now,
                    OrderNumber = "12345",
                    User = user,
                    Items = new List<OrderItem>()
                    {
                        new OrderItem(){
                            Product = products.First(),
                            Quantity = 5,
                            UnitPrice = products.First().Price
                        }
                    }
                };

                _ctx.Orders.Add(order);
                _ctx.SaveChanges();
            }
        }
    }
}
