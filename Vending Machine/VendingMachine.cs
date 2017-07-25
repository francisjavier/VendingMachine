using System;
using System.Collections.Generic;
using System.Linq;

namespace Vending_Machine
{
    public class VendingMachine
    {
        private List<Money> _currentMoney;
        private List<Money> _changeAvailable;
        private IProductProvider _productProvider;
        private IMoneyConverter _moneyConverter;

        public VendingMachine(IMoneyConverter moneyConverter, IProductProvider productProvider)
        {
            if (moneyConverter == null) throw new NotImplementedException("Please provide an implementation for the IMoneyConverted interface");
            if (productProvider == null) throw new NotImplementedException("Please provide an implementation for the IProductProvider interface");

            _moneyConverter = moneyConverter;
            _productProvider = productProvider;

            _currentMoney = new List<Money>();

            //Starting change per requirements (2 quarters, 5 dimes, 3 nickels)
            _changeAvailable = new List<Money>
            {
                new Money { Type = Change.Quarter},
                new Money {Type = Change.Quarter },
                new Money {Type = Change.Dime },
                new Money {Type = Change.Dime },
                new Money {Type = Change.Dime },
                new Money {Type = Change.Dime },
                new Money {Type = Change.Dime },
                new Money {Type = Change.Nickel },
                new Money {Type = Change.Nickel },
                new Money {Type = Change.Nickel },
            };
        }

        public double GetCurrentTotalEntered()
        {
            return _currentMoney.Sum(m => m.Value);
        }

        public Dictionary<string, int> CheckCurrentChangeAvailable()
        {
            return ConvertChange(GetCurrentChange());
        }

        public Status AcceptMoney(string value)
        {
            Status status;

            var money = _moneyConverter.ConvertInput(value);

            if (money.Type != Change.Invalid)
            {
                _currentMoney.Add(money);
                status = Status.ValidMoneyAmount;
            }
            else
            {
                status = Status.InvalidMoneyAmount;
            }

            return status;
        }

        public void ClearAmountEntered()
        {
            _currentMoney.Clear();
        }

        public ProductResult AcceptProductInput(string productIput)
        {
            productIput = productIput?.Replace("#", "");
            var result = new ProductResult();
            int productId;
            var validInteger = int.TryParse(productIput, out productId);

            if (validInteger)
            {
                var products = _productProvider.GetProducts();
                var product = products.FirstOrDefault(p => p.Id == productId);

                if (product == null)
                {
                    result.Status = Status.InvalidProductOption;
                }
                else
                {
                    //product found, try to purchase
                    var changeResult = CheckForEnoughMoneyAndAvailableChange(product.Price);
                    if (changeResult.Status == Status.ChangeAvailable)
                    {
                        result = new ProductResult
                        {
                            ProductName = product.Name,
                            ProductPrice = product.Price,
                            AmountEntered = GetCurrentTotalEntered(),
                            Change = ConvertChange(changeResult.Change),
                            Status = Status.ProductSold
                        };

                        EliminateChangeFromMachine(changeResult.Change);
                        AddChangeToMachine();
                    }
                    else
                    {
                        result.Status = changeResult.Status;
                    }
                }
            }
            else
            {
                result.Status = Status.InvalidProductOption;
            }

            return result;
        }

        private void AddChangeToMachine()
        {
            foreach (var m in _currentMoney)
            {
                if (m.Type != Change.Dollar)
                {
                    _changeAvailable.Add(m);
                }
            }
            _currentMoney.Clear();
        }

        //Aggregates a list of money into key/count (e.g. nickel, 3) pairs that are easier to consume
        //by the client application
        private Dictionary<string, int> ConvertChange(List<Money> change)
        {
            var changeDictionary = new Dictionary<string, int>();

            foreach (var c in change)
            {
                var changeName = c.Type.ToString();
                if (changeDictionary.ContainsKey(changeName))
                {
                    changeDictionary[changeName]++;
                }
                else
                {
                    changeDictionary.Add(changeName, 1);
                }
            }

            return changeDictionary;
        }

        private void EliminateChangeFromMachine(List<Money> change)
        {
            _changeAvailable = _changeAvailable.Except(change).ToList();
        }

        private ChangeResult CheckForEnoughMoneyAndAvailableChange(double productPrice)
        {
            var result = new ChangeResult();

            //Check for enough money to buy
            var currentTotal = GetCurrentTotalEntered();
            if (currentTotal < productPrice)
            {
                result.Status = Status.NotEnoughMoney;
                return result;
            }

            //Check for available change to return
            result.Change = new List<Money>();
            var changeDue = Math.Round(currentTotal - productPrice, 2);
            List<Money> changeAvailable = GetCurrentChange();
            foreach (var changeUnit in changeAvailable)
            {
                if (changeDue - changeUnit.Value >= 0)
                {
                    result.Change.Add(changeUnit);
                    changeDue = Math.Round(changeDue - changeUnit.Value, 2);
                    if (changeDue == 0.00)
                        break;
                }
            }

            result.Status = changeDue == 0.00 ? Status.ChangeAvailable : Status.ChangeNotAvailable;

            return result;
        }

        private List<Money> GetCurrentChange()
        {
            return _changeAvailable.OrderByDescending(o => o.Value).ToList();
        }
    }

    //Returns the change information if change is available
    public class ChangeResult
    {
        public Status Status { get; set; }
        public List<Money> Change { get; set; }
    }

    //Returns product information and change information if sale was successfull
    public class ProductResult
    {
        public Status Status { get; set; }
        public string ProductName { get; set; }
        public double ProductPrice { get; set; }
        public double AmountEntered { get; set; }
        public Dictionary<string, int> Change { get; set; }
    }

    public enum Status
    {
        ValidMoneyAmount,
        InvalidMoneyAmount,
        InvalidProductOption,
        NotEnoughMoney,
        ChangeAvailable,
        ChangeNotAvailable,
        ProductSold
    }
}