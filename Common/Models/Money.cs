using Common.Helpers;
using Common.Models.Enumeration;

namespace Common.Models
{
    public class Money
    {

        public decimal Amount { get; set; }
        public Currency Currency { get; set; }

        public Money(){}


        public Money(decimal amount, Currency currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public static Money operator +(Money a, Money b)
        {
            var aDollar = CurrencyValues.Instance.ConvertMoneyToDollarValue(a);
            var bDollar = CurrencyValues.Instance.ConvertMoneyToDollarValue(b);
            var result = CurrencyValues.Instance.ConvertMoneyFromDollarValue(aDollar + bDollar, a.Currency);
            return result;
        }
        public static Money operator -(Money a, Money b)
        {
            var aDollar = CurrencyValues.Instance.ConvertMoneyToDollarValue(a);
            var bDollar = CurrencyValues.Instance.ConvertMoneyToDollarValue(b);
            var result = CurrencyValues.Instance.ConvertMoneyFromDollarValue(aDollar - bDollar, a.Currency);
            return result;
        }

        public static bool operator <(Money a, Money b)
        {
            var aDollar = CurrencyValues.Instance.ConvertMoneyToDollarValue(a);
            var bDollar = CurrencyValues.Instance.ConvertMoneyToDollarValue(b);
            return aDollar < bDollar ? true : false;
        }
        public static bool operator >(Money a, Money b)
        {
            var aDollar = CurrencyValues.Instance.ConvertMoneyToDollarValue(a);
            var bDollar = CurrencyValues.Instance.ConvertMoneyToDollarValue(b);
            return aDollar > bDollar ? true : false;
        }
    }
}
