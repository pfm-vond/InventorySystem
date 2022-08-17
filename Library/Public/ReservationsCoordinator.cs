using System.Collections.Generic;

namespace ProductReservationSystem.Public
{
    public interface ReservationsCoordinator
    {
        Reservation Hold(List<Stock> stocks);

        List<Reservation> PaginatedReservationBook(int cursor, int limit);
    }
}
