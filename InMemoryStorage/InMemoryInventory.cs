using ProductReservationSystem.Public;

namespace ProductReservationSystem.InMemoryStorage
{
    public class InMemoryInventory : Public.InventoryRepository
    {
        private List<Public.Stock> _stocks = new List<Public.Stock>();
        private List<Public.Stock> _reservedStocks = new List<Public.Stock>();
        private List<Public.Reservation> _reservations = new List<Public.Reservation>();
        private int currentPriority = 0;

        public IQueryable<Public.Stock> Stocks => _stocks.AsQueryable();

        public IQueryable<Public.Reservation> Reservations => _reservations.AsQueryable();

        public IQueryable<Public.Stock> ReservedStocks => _reservedStocks.AsQueryable();

        public void Add(Public.Stock item)
        {
            _stocks.Add(item);
        }

        public void Add(Public.Reservation item)
        {
            item.Priority = currentPriority++;
            item.Id = item.Priority.ToString();
            foreach(var reservedwish in item.Orderlines)
            {
                var reserved = _reservedStocks.SingleOrDefault(s => s.ProductId == reservedwish.ProductId);
                if(reserved is null)
                {
                    _reservedStocks.Add((Stock)reservedwish.Clone());
                }
                else
                {
                    reserved.Quantity += reservedwish.Quantity;
                }
            }

            _reservations.Add(item);
        }
    }
}