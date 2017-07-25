namespace Vending_Machine
{
    public class Money
    {
        public Change Type { get; set; }

        public double Value
        {
            get
            {
                switch (Type)
                {
                    case Change.Nickel:
                        return 0.05;

                    case Change.Dime:
                        return 0.10;

                    case Change.Quarter:
                        return 0.25;

                    case Change.Dollar:
                        return 1.00;

                    case Change.Invalid:
                        return 0;
                }

                return 0;
            }
        }
    }

    public enum Change
    {
        Invalid,
        Nickel,
        Dime,
        Quarter,
        Dollar
    }
}