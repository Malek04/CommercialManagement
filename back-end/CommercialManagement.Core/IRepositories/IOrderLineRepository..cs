using CommercialManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommercialManagement.Core.IRepositories
{
    public interface IOrderLineRepository
    {
        IEnumerable<OrderLine> GetOrderLine();
        OrderLine GetOrderLineById(Guid id);
        void AddOrderLine(OrderLine orderLine);
        void UpdateOrderLine(OrderLine orderLine);
        void DeleteOrderLine(OrderLine orderLine);
    }
}
