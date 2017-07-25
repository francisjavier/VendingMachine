using System;
using System.Collections.Generic;
using Vending_Machine;

namespace Vending_Machine_Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var inputConverter = new DefaultMoneyConverter();
            var productProvider = new ProductRepository();
            var vendingMachine = new VendingMachine(inputConverter, productProvider);

            #region display products on screen
            var products = productProvider.GetProducts();
            foreach (var p in products)
            {
                Console.WriteLine($"Product #{p.Id}   {p.Name.PadRight(13)}   {p.Price:C}");
            }
            #endregion

            do
            {
                var currentChangeString = GetChangeString(vendingMachine.CheckCurrentChangeAvailable());
                var currentTotal = vendingMachine.GetCurrentTotalEntered();
                Console.WriteLine($"\nEnter product number or insert money\n");
                Console.WriteLine($"Current total is:          {currentTotal:C}");
                Console.WriteLine($"Current machine change is: {currentChangeString}\n");

                var input = Console.ReadLine() ?? "";

                //# is a flag to process products, as long as it's not entered, consider the input as money
                if (input.Equals("clear", StringComparison.OrdinalIgnoreCase))
                {
                    vendingMachine.ClearAmountEntered();
                }
                else if (!input.StartsWith("#"))
                {
                    var status = vendingMachine.AcceptMoney(input);

                    if (status != Status.ValidMoneyAmount)
                    {
                        Console.WriteLine("\nPlease enter a valid money option");
                    }
                }
                else
                {
                    var result = vendingMachine.AcceptProductInput(input);

                    switch (result.Status)
                    {
                        case Status.ProductSold:
                            var changeString = GetChangeString(result.Change);

                            Console.WriteLine($"\n{result.ProductName} sold for {result.ProductPrice:C},"
                                              + $" amount entered {result.AmountEntered:C}.");
                            if (!string.IsNullOrEmpty(changeString))
                                Console.WriteLine($"\nChange is {changeString}");
                            break;

                        case Status.InvalidProductOption:
                            Console.WriteLine("\nProduct number not valid");
                            break;

                        case Status.NotEnoughMoney:
                            Console.WriteLine("\nNot enough money to buy product");
                            break;

                        case Status.ChangeNotAvailable:
                            Console.WriteLine("\nNot enough change available to process purchase");
                            break;
                    }
                }
            } while (true);
        }

        //Helper method to convert the dictionary with change values from the vending machine
        //To a message the user can read
        private static string GetChangeString(Dictionary<string, int> result)
        {
            var changeString = string.Empty;
            foreach (var s in result)
            {
                changeString += $"{s.Key} = {s.Value}, ";
            }

            var commaIndex = changeString.LastIndexOf(", ");
            if (commaIndex >= 0)
                changeString = changeString.Remove(commaIndex, 2);

            return changeString;
        }
    }
}