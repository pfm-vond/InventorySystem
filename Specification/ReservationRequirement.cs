using FluentAssertions;
using ProductReservationSystem.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ProductReservationSystem
{
    [Trait("Category", "reservations")]
    public class ReservationRequirement
    {
        [Fact(DisplayName = "A reservation can be created by asking the reservations coordinator to hold items")]
        public void A_reservation_can_be_created_by_asking_the_reservations_coordinator_to_hold_items()
        {
            var inventory = new Program(Program.SampleConfig(Storage.UseInMemory));
            inventory.Stocks["1"] = 10;
            inventory.Stocks["2"] = 20;
            inventory.Stocks["3"] = 30;
            inventory.Stocks["4"] = 40;

            ReservationsCoordinator reservationsCoordinator = inventory.ReservationsCoordinator;

            var myReservation = reservationsCoordinator.Hold(new List<Stock>
            {
                new Stock("1", 5)
            });

            myReservation.CanBeOrder.Should().BeTrue();

            var reservations = reservationsCoordinator.PaginatedReservationBook(0, 5);

            reservations.Should().HaveCount(1)
                .And.Contain(myReservation);
        }


        [Fact(DisplayName = "A reservation can be order only if the inventory has enough products")]
        public void A_reservation_can_be_order_only_if_the_inventory_has_enough_products()
        {
            var inventory = new Program(Program.SampleConfig(Storage.UseInMemory));
            inventory.Stocks["1"] = 10;
            inventory.Stocks["2"] = 20;
            inventory.Stocks["3"] = 30;
            inventory.Stocks["4"] = 40;

            ReservationsCoordinator reservationsCoordinator = inventory.ReservationsCoordinator;

            var myReservation = reservationsCoordinator.Hold(new List<Stock>
            {
                new Stock("1", 5),
                new Stock("2", 15),
                new Stock("3", 1),
                new Stock("4", 40),
            });

            myReservation.CanBeOrder.Should().BeTrue();
        }

        [Fact(DisplayName = "When a reservation exists updating a reserved product Update all reservation status")]
        public void When_a_reservation_exists_updating_a_reserved_product_Update_all_reservation_status()
        {
            var inventory = new Program(Program.SampleConfig(Storage.UseInMemory));
            inventory.Stocks["1"] = 10;
            inventory.Stocks["2"] = 20;
            inventory.Stocks["3"] = 30;
            inventory.Stocks["4"] = 40;

            ReservationsCoordinator reservationsCoordinator = inventory.ReservationsCoordinator;

            var myReservation = reservationsCoordinator.Hold(new List<Stock>
            {
                new Stock("1", 5)
            });

            myReservation.CanBeOrder.Should().BeTrue();

            inventory.Stocks["1"] = 2;

            myReservation.CanBeOrder.Should().BeFalse();
        }

        [Fact(DisplayName = "The identifiers for reservation must be unique")]
        public void The_identifiers_of_reservation_must_be_unique()
        {
            var inventory = new Program(Program.SampleConfig(Storage.UseInMemory));
            inventory.Stocks["1"] = 1;

            ReservationsCoordinator reservationsCoordinator = inventory.ReservationsCoordinator;

            var reservation1 = reservationsCoordinator.Hold(new List<Stock>
            {
                new Stock("1", 5)
            });

            var reservation2 = reservationsCoordinator.Hold(new List<Stock>
            {
                new Stock("1", 3)
            });
            reservation1.Id.Should().NotBeNullOrEmpty();
            reservation2.Id.Should().NotBeNullOrEmpty();
            reservation1.Id.Should().NotBe(reservation2.Id);
        }

        [Fact(DisplayName = "The client can make reservations even if the product is out-of-stock")]
        public void The_client_can_make_reservations_even_if_the_product_is_outOfStock()
        {
            var inventory = new Program(Program.SampleConfig(Storage.UseInMemory));
            inventory.Stocks["1"] = 0;

            ReservationsCoordinator reservationsCoordinator = inventory.ReservationsCoordinator;

            var myReservation = reservationsCoordinator.Hold(new List<Stock>
            {
                new Stock("1", 5)
            });

            myReservation.Should().NotBeNull();
        }

        [Fact(DisplayName = "The reservations must be FIFO")]
        public void The_reservations_must_be_FIFO()
        {
            var inventory = new Program(Program.SampleConfig(Storage.UseInMemory));
            inventory.Stocks["1"] = 6;

            ReservationsCoordinator reservationsCoordinator = inventory.ReservationsCoordinator;

            var reservation1 = reservationsCoordinator.Hold(new List<Stock>
            {
                new Stock("1", 5)
            });

            var reservation2 = reservationsCoordinator.Hold(new List<Stock>
            {
                new Stock("1", 3)
            });

            reservation1.CanBeOrder.Should().BeTrue();
            reservation2.CanBeOrder.Should().BeFalse();
        }

        [Fact(DisplayName = "A product can only be reserved once within the same reservation")]
        public void A_product_can_only_be_reserved_once_within_the_same_reservation()
        {
            var inventory = new Program(Program.SampleConfig(Storage.UseInMemory));
            inventory.Stocks["1"] = 6;

            ReservationsCoordinator reservationsCoordinator = inventory.ReservationsCoordinator;

            Action Reserve = () => reservationsCoordinator.Hold(new List<Stock>
            {
                new Stock("1", 5),
                new Stock("1", 2)
            });

            Reserve.Should().Throw<Exception>()
                .Which.GetBaseException()
                .Should().BeOfType<ArgumentException>()
                .Which.Message.Should().Be("multiple product has the same product Id : 1");
        }

        [Fact(DisplayName = "It is not possible to order an unknown product")]
        public void It_is_not_possible_to_order_an_unknown_product()
        {
            var inventory = new Program(Program.SampleConfig(Storage.UseInMemory));

            ReservationsCoordinator reservationsCoordinator = inventory.ReservationsCoordinator;

            Action Reserve = () => reservationsCoordinator.Hold(new List<Stock>
            {
                new Stock("unknow", 5)
            });

            Reserve.Should().Throw<Exception>()
                .Which.GetBaseException()
                .Should().BeOfType<ArgumentException>()
                .Which.Message.Should().Be("missing items in inventory for reservation : unknow");
        }
    }
}
