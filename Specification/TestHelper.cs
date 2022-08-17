using Autofac;
using Microsoft.Extensions.Configuration;
using Moq;
using ProductReservationSystem.Public;
using System.Collections.Generic;
using System.Linq;

namespace ProductReservationSystem
{
    internal static class TestHelper
    {
        public static void MockVeryLargeDb(ContainerBuilder cb, IConfiguration config)
        {
            var repoMock = new Mock<InventoryRepository>();

            repoMock.Setup(s => s.Stocks)
                .Returns(VeryLongDb()
                    .AsQueryable());

            cb.RegisterInstance(repoMock.Object)
                .As<InventoryRepository>();
        }

        private static IEnumerable<Stock> VeryLongDb()
        {
            uint i = 0;
            while (true)
                yield return new Stock($"id{i}", i++);
        }
    }
}
