using System;

namespace Vending_Machine
{
    //Extracted this to an interface so that the VendingMachine class doesn't have to worry about
    //the input to money conversion

    public class DefaultMoneyConverter : IMoneyConverter
    {
        public Money ConvertInput(string input)
        {
            var money = new Money();

            if (input.Equals("N", StringComparison.OrdinalIgnoreCase))
            {
                money.Type = Change.Nickel;
            }
            else if (input.Equals("D", StringComparison.OrdinalIgnoreCase))
            {
                money.Type = Change.Dime;
            }
            else if (input.Equals("Q", StringComparison.OrdinalIgnoreCase))
            {
                money.Type = Change.Quarter;
            }
            else if (input.Equals("B", StringComparison.OrdinalIgnoreCase))
            {
                money.Type = Change.Dollar;
            }
            else //invalid input
            {
                money.Type = Change.Invalid;
            }

            return money;
        }
    }
}