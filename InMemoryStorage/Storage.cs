using Autofac;
using Microsoft.Extensions.Configuration;
using ProductReservationSystem.InMemoryStorage;

namespace ProductReservationSystem
{
    public static partial class Storage
    {
        public static void UseInMemory(ContainerBuilder cb, IConfiguration config)
        {
            cb.RegisterType<InMemoryInventory>()
                .As<Public.InventoryRepository>()
                .SingleInstance();
        }

    }
}
