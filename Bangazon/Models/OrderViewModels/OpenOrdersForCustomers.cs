using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon.Models.OrderViewModels
{
    public class OpenOrdersForCustomers
    {

        public int OrderId { get; set; }
        public Order Order { get; set; }
        public string Label { get; set; }
        public double Quantity { get; set; }
        public int Count { get; set; }

    }
}
