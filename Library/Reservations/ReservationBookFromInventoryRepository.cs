using ProductReservationSystem.Exceptions;
using ProductReservationSystem.Inventories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProductReservationSystem.Reservations
{
    internal class ReservationBookFromInventoryRepository : ReservationBook, ReactToAddOrUpdateStock
    {
        private Public.InventoryRepository Storage { get; }
        private ArgumentExceptionFactory.MissingItems MissingProduct { get; }

        public ReservationBookFromInventoryRepository(
            Public.InventoryRepository storage,
            ArgumentExceptionFactory.MissingItems missingProduct
            )
        {
            Storage = storage;
            MissingProduct = missingProduct;
        }

        public IQueryable<Public.Reservation> Skip(int cursor)
        {
            return Storage.Reservations.Skip(cursor);
        }

        public void Write(Public.Reservation reservation)
        {
            var reservedIds = reservation.Orderlines.Select(s => s.ProductId);

            var stockToUpdate = Storage.Stocks
                .Where(s => reservedIds.Contains(s.ProductId))
                .ToDictionary(s => s.ProductId);

            var articleNotInCatalog = reservation.Orderlines
                .Where(ol => !stockToUpdate.ContainsKey(ol.ProductId));

            if (articleNotInCatalog.Any())
            {
                throw MissingProduct(articleNotInCatalog.Select(s => s.ProductId));
            }

            Storage.Add(reservation);
            
            var reservedStocks = Storage.ReservedStocks
                .Where(s => reservedIds.Contains(s.ProductId))
                .ToDictionary(s => s.ProductId);
            
            UpdateCanBeOrder(reservation, stockToUpdate, reservedStocks);
        }

        private void UpdateCanBeOrder(Public.Reservation reservation, Dictionary<string, Public.Stock> stockToUpdate, Dictionary<string, Public.Stock> reservedStocks)
        {
            reservation.CanBeOrder = true;
            foreach (Public.Stock item in reservation.Orderlines)
            {
                if (reservedStocks[item.ProductId].Quantity > stockToUpdate[item.ProductId].Quantity)
                {
                    reservation.CanBeOrder = false;
                    break;
                }
            }
        }

        void ReactToAddOrUpdateStock.Raise(string productId, uint quantity)
        {
            var reservations = Storage.Reservations
                .Where(r => r.Orderlines.Any(ol => ol.ProductId == productId))
                .OrderBy(r => r.Priority)
                .ToList();

            if (reservations.Any())
            {
                var reservedStock = Storage.ReservedStocks.Single(rp => rp.ProductId == productId);
                reservedStock.Quantity = 0;

                foreach(var reservation in reservations)
                {
                    reservedStock.Quantity += reservation.Orderlines.Single(r => r.ProductId == productId).Quantity;

                    if (reservedStock.Quantity < quantity)
                        reservation.CanBeOrder = true;
                    else
                        reservation.CanBeOrder = false;
                }
            }

        }
    }
}
