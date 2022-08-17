using ProductReservationSystem.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductReservationSystem.Public
{
    public class Reservation
    {
        public Reservation()
        {
            Orderlines = new List<Stock>();
        }

        public Reservation(ArgumentExceptionFactory.DuplicatesProduct DuplicatesProductId, List<Stock> orderlines)
        {
            var duplication = orderlines.GroupBy(o => o.ProductId).Where(g => g.Count() > 1);
            if (duplication.Any())
                throw DuplicatesProductId(duplication.Select(s => s.Key));
            Orderlines = orderlines;
        }

        public List<Stock> Orderlines { get; set; }

        public bool CanBeOrder { get; set; }
        public int Priority { get; set; }
        public string Id { get; set; }
    }
}
