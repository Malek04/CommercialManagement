using CommercialManagement.Core.Enums;
using System;
using System.Collections.Generic;

namespace CommercialManagement.Core.DTOs
{
    public class OrderDto
    {
        // Order
        public Guid Id { get; set; }
        public string? OrderNumber { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Draft;
        public decimal TotalHT { get; set; }
        public decimal TotalTTC { get; set; }

        // Client
        public Guid ClientId { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime Created { get; set; }

        // Address
        public string? Rue { get; set; }
        public string? Ville { get; set; }
        public string? CodePostal { get; set; }
        public string? Pays { get; set; }

        // Fusion : les lignes vivent maintenant dans la commande
        public List<OrderLineItemDto> Lines { get; set; } = new();
    }

    // Représentation d'une ligne dans la réponse d'une commande
    public class OrderLineItemDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string? ProductReference { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalLine => Quantity * UnitPrice;
        public int ProductStockQuantity { get; set; }
    }

    // Body pour POST/PUT api/orders
    public class OrderRequestDto
    {
        public Guid ClientId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public List<OrderLineRequestDto> Lines { get; set; } = new();
    }

    public class OrderLineRequestDto
    {
        // null = nouvelle ligne à créer, sinon Id d'une ligne existante à mettre à jour
        public Guid? Id { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}