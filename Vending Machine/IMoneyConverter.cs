namespace Vending_Machine
{
    //Allows a way to externally specify the money input to the vending machine
    public interface IMoneyConverter
    {
        Money ConvertInput(string input);
    }
}