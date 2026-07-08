using CommercialManagement.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CommercialManagement.Core.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public string? OrderNumber { get; set; }
        public Guid ClientId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Draft;

        public decimal TotalHT { get; set; }
        public decimal TotalTTC { get; set; }

        [JsonIgnore]
        public Client? Client { get; set; }

        public ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();

        // Méthode de calcul des totaux
        public void CalculateTotals()
        {
            TotalHT = OrderLines.Sum(ol => ol.TotalLine);
            TotalTTC = TotalHT * 1.19m;
        }

    }
}
