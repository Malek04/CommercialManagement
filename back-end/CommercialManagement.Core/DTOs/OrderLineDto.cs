using CommercialManagement.Core.Enums;
using CommercialManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CommercialManagement.Core.DTOs
{
    public class OrderLineDto
    {
        //orderLine
        public Guid Id { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalLine { get; set; }
        //order
        public Guid OrderId { get; set; }

        public string? OrderNumber { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public OrderStatus Status { get; set; } = OrderStatus.Draft;

        public decimal TotalHT { get; set; }

        public decimal TotalTTC { get; set; }

        // Client
        public Guid Client_Id { get; set; }

        public string? Client_LastName { get; set; }

        public string? Client_FirstName { get; set; }

        public string? Client_Email { get; set; }

        public string? Client_Phone { get; set; }

        public DateTime Client_Created { get; set; }

        // Address
        public string? Adresse_Rue { get; set; }

        public string? Adresse_Ville { get; set; }

        public string? Adresse_CodePostal { get; set; }

        public string? Adresse_Pays { get; set; }
        //product
        public Guid Product_Id { get; set; }
        public string? Product_Reference { get; set; }

        public string? Product_Name { get; set; }

        public string? Product_Description { get; set; }

        public decimal Product_UnitPriceHT { get; set; }

        public int Product_StockQuantity { get; set; }

        public DateTime Product_Created { get; set; } = DateTime.UtcNow;

    }
}
