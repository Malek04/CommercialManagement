using CommercialManagement.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

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

        // TVA fixe à 19% 
        private const decimal VatRate = 1.19m;

        [JsonIgnore]
        public Client? Client { get; set; }

        public ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();

        public void CalculateTotals()
        {
            TotalHT = OrderLines.Sum(ol => ol.TotalLine);
            TotalTTC = TotalHT * VatRate;
        }
    }
}