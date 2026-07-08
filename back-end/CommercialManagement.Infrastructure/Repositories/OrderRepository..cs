using CommercialManagement.Core.IRepositories;
using CommercialManagement.Core.Models;
using CommercialManagement.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommercialManagement.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly CommercialManagementDbContext _context;

        public OrderRepository(CommercialManagementDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Order> GetOrder()
        {
            return _context.Orders
                .Include(o => o.Client)
                    .ThenInclude(c => c!.Adresse)
                .Include(o => o.OrderLines)
                    .ThenInclude(ol => ol.Product)
                .AsNoTracking()
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        public Order? GetOrderById(Guid id)
        {
            return _context.Orders
                .Include(o => o.Client)
                    .ThenInclude(c => c!.Adresse)
                .Include(o => o.OrderLines)
                    .ThenInclude(ol => ol.Product)
                .FirstOrDefault(o => o.Id == id);
        }

        public void AddOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public void UpdateOrder(Order order)
        {
            _context.Orders.Update(order);
            _context.SaveChanges();
        }

        public void DeleteOrder(Order order)
        {
            _context.Orders.Remove(order);
            _context.SaveChanges();
        }
    }
}