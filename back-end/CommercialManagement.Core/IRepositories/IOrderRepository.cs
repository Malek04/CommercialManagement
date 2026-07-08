using CommercialManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommercialManagement.Core.IRepositories
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetOrder();
        Order GetOrderById(Guid id);
        void AddOrder(Order order);
        void UpdateOrder(Order order);
        void DeleteOrder(Order order);
    }
}
