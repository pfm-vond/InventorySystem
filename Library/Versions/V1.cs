using Autofac;
using Autofac.Versioned;
using System;

namespace ProductReservationSystem.Versions
{
    using Features;

    internal class V1 : Module, IVersionModule,
        IIncludeFeature<InventoryCountFeature>,
        IIncludeFeature<ReservationFeature>
    { 
        public Version AvailableSince => new Version(1, 0);

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

        }
    }
}
