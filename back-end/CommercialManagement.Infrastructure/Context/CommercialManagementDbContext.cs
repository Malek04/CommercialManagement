using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using CommercialManagement.Core.Models;

namespace CommercialManagement.Infrastructure.Context
{
    public class CommercialManagementDbContext: DbContext
    {
        public CommercialManagementDbContext()
        {
        }
        public CommercialManagementDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Client> Clients { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderLine> OrderLines { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
    }
}
