using Autofac;
using Autofac.Versioned;
using ProductReservationSystem.Inventories;
using System.Collections.Generic;

namespace ProductReservationSystem.Versions.Features
{
    internal class InventoryCountFeature : Module, IFeatureModule, IFeatureDescription
    {
        public string FeatureName => "Inventory Count";

        public bool ActiveByDefault => true;

        public string Description => "User can count product in the warehouse and fill the total into the system and retrieve it back";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<InventoryCountFromInventoryRepository>()
                .As<Public.InventoryCount>()
                .InstancePerDependency();


            builder.Register((string s) => new KeyNotFoundException(s))
                .AsSelf()
                .InstancePerDependency();


            builder.Register((string id, uint quantity) => new Public.Stock
                {
                    ProductId = id,
                    Quantity = quantity,
                })
                .AsSelf()
                .InstancePerDependency();
        }
    }
}