using CommercialManagement.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommercialManagement.Core.Models
{
    public class Order
    {
        public Guid Id { get; set; }

        public string? OrderNumber { get; set; }

        public Guid ClientId { get; set; }

        public Client Client { get; set; } = null!;

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public OrderStatus Status { get; set; } = OrderStatus.Draft;

        public decimal TotalHT { get; set; }

        public decimal TotalTTC { get; set; }

        // Navigation Property
        public ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
    }
}
