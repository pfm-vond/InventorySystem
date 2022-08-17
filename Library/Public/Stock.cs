using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductReservationSystem.Public
{
    public class Stock : ICloneable
    {
        public Stock(string productId, uint quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }

        public Stock()
        {

        }

        public string ProductId { get; set; }

        public uint Quantity { get; set; }

        public object Clone()
        {
            return new Stock(ProductId, Quantity);
        }
    }
}
