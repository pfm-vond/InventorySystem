using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductReservationSystem.Exceptions
{
    public static class ArgumentExceptionFactory
    {
        public delegate ArgumentException MissingItems(IEnumerable<string> missingItems);
        public delegate ArgumentException DuplicatesProduct(IEnumerable<string> missingItems);

        public static ArgumentException DuplicatesProductFactory(IEnumerable<string> duplicatesItems)
        {
            return new ArgumentException(
                    $"multiple product has the same product Id : " +
                    $"{string.Join(",", duplicatesItems)}");
        }

        public static ArgumentException MissingItemsFactory(IEnumerable<string> missingItems)
        {
            return new ArgumentException(
                    $"missing items in inventory for reservation : " +
                    $"{string.Join(",", missingItems)}");
        }
    }
}
