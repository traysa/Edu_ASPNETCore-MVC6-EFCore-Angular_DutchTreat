using System.Collections.Generic;
using DutchTreat.Data.Entities;

namespace DutchTreat.Data
{
    /// <summary>
    /// An interface of the Dutch Repository is created to be able to do mocking;
    /// For testing a dutch repository, which returns static data can be used instead of making database calls
    /// </summary>
    public interface IDutchRepository
    {
        IEnumerable<Product> GetAllProducts();
        IEnumerable<Product> GetProductsByCategory(string category);

        bool SaveAll();
    }
}