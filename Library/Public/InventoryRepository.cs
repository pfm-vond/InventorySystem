using System.Collections.Generic;
using System.Linq;

namespace ProductReservationSystem.Public
{
    public interface InventoryRepository
    {
        IQueryable<Stock> Stocks { get; }
        IQueryable<Reservation> Reservations { get; }
        IQueryable<Stock> ReservedStocks { get; }

        void Add(Stock item);
        void Add(Reservation item);
    }
}
