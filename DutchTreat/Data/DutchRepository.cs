using DutchTreat.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DutchTreat.Data
{
    /// <summary>
    /// Repositary Pattern:
    /// - Expose the different calls to the database needed
    /// - Supports testability
    /// </summary>
    public class DutchRepository : IDutchRepository
    {
        private readonly DutchContext _ctx;
        private readonly ILogger<DutchRepository> _logger;

        /// <summary>
        /// Constructor where a DutchContext is injected
        /// </summary>
        /// <param name="ctx"></param>
        public DutchRepository(DutchContext ctx, ILogger<DutchRepository> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        // Save an entity to the db
        public void AddEntity(object model)
        {
            _ctx.Add(model);
        }

        public IEnumerable<Order> GetAllOrders(bool includeitems)
        {
            if (includeitems)
            {
                return _ctx.Orders
                        .Include(o => o.Items) // Includes items of the order
                        .ThenInclude(i => i.Product) // Includes product of the items
                        .ToList();
                // To not get the following error: "Self referencing loop detected for property 'order'"
                // add Json options in services.AddMvc() in the configuration services in Startup.cs.

            }
            else
            {
                return _ctx.Orders.ToList(); //Return orders but without items they contain

            }
        }

        public IEnumerable<Order> GetAllOrdersByUser(string username, bool includeitems)
        {
            if (includeitems)
            {
                return _ctx.Orders
                        .Where(o => o.User.UserName == username) // Only orders from a specific user
                        .Include(o => o.Items) // Includes items of the order
                        .ThenInclude(i => i.Product) // Includes product of the items
                        .ToList();
                // To not get the following error: "Self referencing loop detected for property 'order'"
                // add Json options in services.AddMvc() in the configuration services in Startup.cs.

            }
            else
            {
                return _ctx.Orders
                        .Where(o => o.User.UserName == username)
                        .ToList(); //Return orders but without items they contain

            }
        }

        public IEnumerable<Product> GetAllProducts()
        {
            try
            {
                _logger.LogInformation("Get all products was called");

                return _ctx.Products
                    .OrderBy(p => p.Title)
                    .ToList();
            } catch (Exception ex)
            {
                _logger.LogError("Failed to get all products: {ex}");
                return null;
            }
        }

        public Order GetOrderById(string username, int id)
        {
            return _ctx.Orders
                        .Include(o => o.Items) // Includes items of the order
                        .ThenInclude(i => i.Product) // Includes product of the items
                        .Where(o => o.Id == id && o.User.UserName == username) // Get specific order for specific user
                        .FirstOrDefault();
            // returns nulls if id does not belong to user
        }

        public IEnumerable<Product> GetProductsByCategory(string category)
        {
            try
            {
                return _ctx.Products
                .Where(p => p.Category == category)
                .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get all products of a category: {ex}");
                return null;
            }
        }

        public bool SaveAll()
        {
            return _ctx.SaveChanges() > 0;
        }
    }
}
