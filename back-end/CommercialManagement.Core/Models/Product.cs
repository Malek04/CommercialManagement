using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommercialManagement.Core.Models
{
    public class Product
    {
        public Guid Id { get; set; }

        public string? Reference { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public decimal UnitPriceHT { get; set; }

        public int StockQuantity { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

    }
}
