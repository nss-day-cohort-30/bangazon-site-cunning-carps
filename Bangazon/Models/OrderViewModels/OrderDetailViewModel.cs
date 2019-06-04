using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bangazon.Models.OrderViewModels
{
    public class OrderDetailViewModel
    {
        public Order Order { get; set; }

        public IEnumerable<OrderLineItem> LineItems { get; set; }

        public List<PaymentType> PaymentTypes { get; set; }
    }
}
