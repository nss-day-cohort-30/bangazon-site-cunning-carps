using System.Collections.Generic;

namespace Bangazon.Models.OrderViewModels
{
    public class OrderDetailViewModel
    {
        public Order Order { get; set; }

        public IEnumerable<OrderLineItem> LineItems { get; set; }

        public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> PaymentTypes { get; set; }

        public string Error { get; set; }
        
    }
}
