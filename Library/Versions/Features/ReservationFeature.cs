using Autofac;
using Autofac.Versioned;
using ProductReservationSystem.Exceptions;
using ProductReservationSystem.Inventories;
using ProductReservationSystem.Reservations;
using System;
using System.Collections.Generic;

namespace ProductReservationSystem.Versions.Features
{
    internal class ReservationFeature : Module, IFeatureModule, IFeatureDescription
    {
        public string FeatureName => "Reservation";

        public bool ActiveByDefault => true;

        public string Description => "User can create a reservation to hold some stock of product for future purchase";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ReservationBookWriter>()
                .As<Public.ReservationsCoordinator>()
                .InstancePerDependency();

            builder.RegisterType<ReservationBookFromInventoryRepository>()
                .As<ReservationBook>()
                .As<ReactToAddOrUpdateStock>()
                .InstancePerDependency();

            builder.RegisterType<Public.Reservation>()
                .AsSelf()
                .InstancePerDependency();

            builder.RegisterInstance<ArgumentExceptionFactory.DuplicatesProduct>(ArgumentExceptionFactory.DuplicatesProductFactory)
                .SingleInstance();

            builder.RegisterInstance<ArgumentExceptionFactory.MissingItems>(ArgumentExceptionFactory.MissingItemsFactory)
                .SingleInstance();
        }
    }
}
