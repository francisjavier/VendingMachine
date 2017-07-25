using System.Collections.Generic;

namespace Vending_Machine
{
    public class ProductRepository : IProductProvider
    {
        public List<Product> GetProducts()
        {
            return new List<Product>
            {
                new Product {Id = 1, Name = "Coke", Price = 0.95 },
                new Product {Id = 2, Name = "Diet Coke", Price = 0.90 },
                new Product {Id = 3, Name = "Candy Bar", Price = 0.60 },
                new Product {Id = 4, Name = "Gum", Price = 0.30 },
                new Product {Id = 5, Name = "Chips", Price = 1.10 },
                new Product {Id = 6, Name = "Energy Drink", Price = 1.00 },
            };
        }
    }
}