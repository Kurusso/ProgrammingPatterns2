using CreditApplication.Helpers;
using CreditApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace CreditApplication.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void ApplyMoneyValueConverter(this ModelBuilder modelBuilder)
        {
            var entityTypeList = modelBuilder.Model.GetEntityTypes().ToList();

            foreach (var entityType in entityTypeList)
            {
                var properties = entityType.ClrType.GetProperties()
                    .Where(p => p.PropertyType == typeof(Money));

                foreach (var property in properties)
                {
                    var entityBuilder = modelBuilder.Entity(entityType.ClrType);
                    var propertyBuilder = entityBuilder.Property(property.PropertyType, property.Name);

                    propertyBuilder.HasConversion(new MoneyValueConverter())
                                   .HasColumnType("nvarchar(max)");
                }
            }
        }
    }
}
