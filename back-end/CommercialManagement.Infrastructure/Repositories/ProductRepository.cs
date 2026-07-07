using CommercialManagement.Core.IRepositories;
using CommercialManagement.Core.Models;
using CommercialManagement.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommercialManagement.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly CommercialManagementDbContext _context;

        public ProductRepository(CommercialManagementDbContext context)
        {
            _context = context;
        }
        public IEnumerable<Product> GetProduct()
        {
            return _context.Products
                           .AsNoTracking()
                           .OrderByDescending(p => p.Created)
                           .ToList();
        }

        public Product? GetProductById(Guid id)
        {
            return _context.Products
                           .AsNoTracking()
                           .FirstOrDefault(c => c.Id == id);
        }

        public void AddProduct(Product product)
        {
            product.Id = Guid.NewGuid();
            product.Created = DateTime.UtcNow;

            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void UpdateProduct(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
        }

        public void DeleteProduct(Product product)
        {
            _context.Products.Remove(product);
            _context.SaveChanges();
        }
    }
}
