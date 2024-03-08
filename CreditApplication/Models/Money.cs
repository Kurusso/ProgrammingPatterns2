using CreditApplication.Helpers;
using CreditApplication.Models.Enumeration;

namespace CreditApplication.Models
{
    public class Money
    {
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }

        public Money() { }


        public Money(decimal amount, Currency currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public static Money operator +(Money a, Money b)
        {
            var aDollar = MoneyConverter.ConvertMoneyToDollarValue(a);
            var bDollar = MoneyConverter.ConvertMoneyToDollarValue(b);
            var result = MoneyConverter.ConvertMoneyFromDollarValue(aDollar + bDollar, a.Currency);
            return result;
        }
        public static Money operator -(Money a, Money b)
        {
            var aDollar = MoneyConverter.ConvertMoneyToDollarValue(a);
            var bDollar = MoneyConverter.ConvertMoneyToDollarValue(b);
            var result = MoneyConverter.ConvertMoneyFromDollarValue(aDollar - bDollar, a.Currency);
            return result;
        }

        public static bool operator <(Money a, Money b)
        {
            var aDollar = MoneyConverter.ConvertMoneyToDollarValue(a);
            var bDollar = MoneyConverter.ConvertMoneyToDollarValue(b);
            return aDollar < bDollar ? true : false;
        }
        public static bool operator >(Money a, Money b)
        {
            var aDollar = MoneyConverter.ConvertMoneyToDollarValue(a);
            var bDollar = MoneyConverter.ConvertMoneyToDollarValue(b);
            return aDollar > bDollar ? true : false;
        }
    }
}
