using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProductReservationSystem.Reservations
{
    internal class ReservationBookWriter : Public.ReservationsCoordinator
    {
        public ReservationBookWriter(
            ReservationBook book,
            Func<List<Public.Stock>, Public.Reservation> createReservation)
        {
            Book = book;
            CreateReservation = createReservation;
        }

        private ReservationBook Book { get; }
        private Func<List<Public.Stock>, Public.Reservation> CreateReservation;

        public Public.Reservation Hold(List<Public.Stock> stocks)
        {
            var reservation = CreateReservation(stocks);
            Book.Write(reservation);
            return reservation;
        }

        public List<Public.Reservation> PaginatedReservationBook(int cursor, int limit)
        {
            return Book.Skip(cursor).Take(limit).ToList();
        }
    }
}
