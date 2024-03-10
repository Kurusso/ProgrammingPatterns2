using Common.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace Common.Helpers
{
    public class MoneyValueConverter : ValueConverter<Money, string>
    {
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public MoneyValueConverter() : base(
            v => ConvertToString(v),
            v => ConvertToMoney(v))
        {}

        private static string ConvertToString(Money value)
        {

            return JsonSerializer.Serialize(value, _jsonOptions);
        }

        private static Money ConvertToMoney(string value)
        {
            return JsonSerializer.Deserialize<Money>(value, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }
}
