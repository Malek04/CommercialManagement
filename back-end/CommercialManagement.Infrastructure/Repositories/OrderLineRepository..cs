using CommercialManagement.Core.IRepositories;
using CommercialManagement.Core.Models;
using CommercialManagement.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommercialManagement.Infrastructure.Repositories
{
    public class OrderLineRepository : IOrderLineRepository
    {
        private readonly CommercialManagementDbContext _context;

        public OrderLineRepository(CommercialManagementDbContext context)
        {
            _context = context;
        }

        public void AddOrderLine(OrderLine orderLine)
        {
            _context.OrderLines.Add(orderLine);
            _context.SaveChanges();
        }

        public void UpdateOrderLine(OrderLine orderLine)
        {
            _context.OrderLines.Update(orderLine);
            _context.SaveChanges();
        }

        public void DeleteOrderLine(OrderLine orderLine)
        {
            _context.OrderLines.Remove(orderLine);
            _context.SaveChanges();
        }

        public OrderLine? GetOrderLineById(Guid id)
        {
            return _context.OrderLines
                .Include(ol => ol.Order)
                    .ThenInclude(o => o!.Client)
                        .ThenInclude(c => c!.Adresse)
                .Include(ol => ol.Product)
                .AsNoTracking()
                .FirstOrDefault(ol => ol.Id == id);
        }

        public IEnumerable<OrderLine> GetOrderLine()
        {
            return _context.OrderLines
                .Include(ol => ol.Order)
                    .ThenInclude(o => o!.Client)
                        .ThenInclude(c => c!.Adresse)
                .Include(ol => ol.Product)
                .AsNoTracking()
                .ToList();
        }
    }
}