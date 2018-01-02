using DutchTreat.Data.Entities;
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
