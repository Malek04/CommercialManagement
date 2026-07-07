using Microsoft.EntityFrameworkCore;

namespace CommercialManagement.Core.Models
{
    public class Client
    {
        public Guid Id { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public Adresse Adresse { get; set; } = new Adresse();
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }

    [Owned]
    public class Adresse
    {
        public string? Rue { get; set; }
        public string? Ville { get; set; }
        public string? CodePostal { get; set; }
        public string? Pays { get; set; }
    }
}