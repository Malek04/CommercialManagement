using CommercialManagement.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CommercialManagement.Core.Models
{
    public class OrderLine
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalLine { get; set; }
        //navigation property
        [JsonIgnore]
        public Order? Order { get; set; }
        [JsonIgnore]
        public Product? Product { get; set; }
    }
}
