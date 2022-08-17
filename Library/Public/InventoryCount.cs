using System.Collections.Generic;

namespace ProductReservationSystem.Public
{
    public interface InventoryCount : InventoryCount<string, string> { }

    public interface InventoryCount<TKey, TProduct>
    {
        public uint this[TKey index] { get; set; }

        public List<Stock> PaginatedStockList(int cursor, int limit);
    }
}