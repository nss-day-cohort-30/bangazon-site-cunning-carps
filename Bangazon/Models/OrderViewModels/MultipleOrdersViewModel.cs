using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon.Models.OrderViewModels
{
    public class MultipleOrdersViewModel
    {
        public Order Order { get; set; }

        public IEnumerable<ApplicationUser> ApplicationUsers { get; set; }

        public string Error { get; set; }

    }
}
