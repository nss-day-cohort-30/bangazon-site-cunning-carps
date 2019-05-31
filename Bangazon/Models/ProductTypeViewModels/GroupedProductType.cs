using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon.Models
{
    public class GroupedProductType
    {
        public int ProductTypeId { get; set; }
        public ProductType ProductType { get; set; }
        public string Label { get; set; }
        public double Quantity { get; set; }
        public int Count { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
