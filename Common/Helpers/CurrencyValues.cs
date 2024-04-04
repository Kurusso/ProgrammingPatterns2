using Common.Models;
using Common.Models.Enumeration;

namespace Common.Helpers
{
    public class CurrencyValues
    {
        private static CurrencyValues instance = null;
        private static object syncRoot = new object();

        private readonly Dictionary<Currency, decimal> CurrencyDictionary = new Dictionary<Currency, decimal>
        {
            [Currency.Dollar] = 1m,
            [Currency.Euro] = 0m,
            [Currency.Ruble] = 0m
        };

        private CurrencyValues() { }

        public void UpdateValues(decimal rubleToDollar, decimal euroToDollar, decimal dollarToDollar)
        {
            lock (syncRoot)
            {
                CurrencyDictionary[Currency.Dollar] = dollarToDollar;
                CurrencyDictionary[Currency.Euro] = euroToDollar;
                CurrencyDictionary[Currency.Ruble] = rubleToDollar;
            }
        }

        public static CurrencyValues Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new CurrencyValues();
                    }
                }

                return instance;
            }
        }

        public decimal ConvertMoneyToDollarValue(Money money)
        {
            decimal currency;
            CurrencyDictionary.TryGetValue(money.Currency, out currency);
            return money.Amount / currency;
        }

        public  Money ConvertMoneyFromDollarValue(decimal moneyAmountToConvert, Currency finalCurrency)
        {
            decimal currency;
            CurrencyDictionary.TryGetValue(finalCurrency, out currency);
            return new Money { Amount = moneyAmountToConvert * currency, Currency = finalCurrency };
        }
    }
}
