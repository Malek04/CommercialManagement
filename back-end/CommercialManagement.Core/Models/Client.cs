using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommercialManagement.Core.Models
{
    public class Client
    {
        public Guid Id { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public Adresse Adresse { get; set; } = new();
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
