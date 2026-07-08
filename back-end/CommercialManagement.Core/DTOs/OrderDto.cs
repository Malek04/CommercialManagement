using CommercialManagement.Core.Enums;

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
    }
}