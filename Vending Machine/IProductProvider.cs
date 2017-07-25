using System.Collections.Generic;

namespace Vending_Machine
{
    //Allows a way to externally provide products to vending machine
    public interface IProductProvider
    {
        List<Product> GetProducts();
    }
}