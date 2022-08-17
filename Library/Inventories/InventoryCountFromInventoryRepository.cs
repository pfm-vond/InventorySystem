using ProductReservationSystem.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductReservationSystem.Inventories
{
    internal class InventoryCountFromInventoryRepository : Public.InventoryCount
    {
        private InventoryRepository Storage { get; }
        private Func<string, KeyNotFoundException> KeyNotFound { get; }
        private Func<string, uint, Stock> CreateStock { get; }
        private List<ReactToAddOrUpdateStock> OnAddOrUpdateStock { get; }

        public InventoryCountFromInventoryRepository(
            Public.InventoryRepository storage,
            IEnumerable<ReactToAddOrUpdateStock> onAddOrUpdateStock,
            Func<string, KeyNotFoundException> keyNotFound,
            Func<string, uint, Stock> createStock)
        {
            Storage = storage;
            KeyNotFound = keyNotFound;
            CreateStock = createStock;
            OnAddOrUpdateStock = onAddOrUpdateStock.ToList();
        }

        public uint this[string productId] 
        {
            get => Storage.Stocks.SingleOrDefault(s => s.ProductId == productId)?.Quantity
                    ?? throw KeyNotFound(productId);
            set
            {
                var quantityToAddOrUpdate = Storage.Stocks.SingleOrDefault(s => s.ProductId == productId);

                if(quantityToAddOrUpdate is null)
                {
                    quantityToAddOrUpdate = CreateStock(productId, value);
                    Storage.Add(quantityToAddOrUpdate);
                }
                else
                {
                    quantityToAddOrUpdate.Quantity = value;
                }

                foreach(var listener in OnAddOrUpdateStock)
                    listener.Raise(productId, value);
            }
        }

        public List<Stock> PaginatedStockList(int cursor, int limit)
        {
            return Storage.Stocks.Skip(cursor).Take(limit).ToList();
        }
    }
}
