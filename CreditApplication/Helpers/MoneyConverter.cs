using CreditApplication.Models;
using CreditApplication.Models.Enumeration;

namespace CreditApplication.Helpers
{
    public class MoneyConverter
    {
        public static decimal RubleToDollar = new decimal(0.010946);
        public static decimal EuroToDollar = new decimal(1.08);
        public static decimal DollarToDollar = new decimal(1);

        private static readonly Dictionary<Currency, decimal> CurrencyDictionary = new Dictionary<Currency, decimal>
        {
            [Currency.Dollar] = MoneyConverter.DollarToDollar,
            [Currency.Euro] = MoneyConverter.EuroToDollar,
            [Currency.Ruble] = MoneyConverter.RubleToDollar,
        };

        public static decimal ConvertMoneyToDollarValue(Money money)
        {
            decimal currency;
            CurrencyDictionary.TryGetValue(money.Currency, out currency);
            return money.Amount * currency;
        }

        public static Money ConvertMoneyFromDollarValue(decimal moneyAmountToConvert, Currency finalCurrency)
        {
            decimal currency;
            CurrencyDictionary.TryGetValue(finalCurrency, out currency);
            return new Money { Amount = moneyAmountToConvert / currency, Currency = finalCurrency };
        }
    }
}
