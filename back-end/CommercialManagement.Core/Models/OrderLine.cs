using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CommercialManagement.Core.Models
{
    public class OrderLine
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La quantité doit être supérieure à 0.")]
        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        // Calcul automatique du total de la ligne (Quantité × Prix unitaire)
        public decimal TotalLine => Quantity * UnitPrice;

        [JsonIgnore]
        public Order? Order { get; set; }
        [JsonIgnore]
        public Product? Product { get; set; }
    }
}