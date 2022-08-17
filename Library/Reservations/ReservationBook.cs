using System.Linq;

namespace ProductReservationSystem.Reservations
{
    internal interface ReservationBook
    {
        IQueryable<Public.Reservation> Skip(int cursor);
        void Write(Public.Reservation reservation);
    }
}
