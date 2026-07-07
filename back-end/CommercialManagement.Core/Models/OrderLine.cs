using CommercialManagement.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommercialManagement.Core.Models
{
    public class OrderLine
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public Order Order { get; set; } = null!;

        public Guid ProductId { get; set; }

        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalLine { get; set; }
    }
}
