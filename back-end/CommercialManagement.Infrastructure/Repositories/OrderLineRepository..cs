using CommercialManagement.Core.IRepositories;
using CommercialManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommercialManagement.Infrastructure.Repositories
{
    public class OrderLineRepository : IOrderLineRepository
    {
        public void AddOrderLine(OrderLine orderLine)
        {
            throw new NotImplementedException();
        }

        public void DeleteOrderLine(OrderLine orderLine)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OrderLine> GetOrderLine()
        {
            throw new NotImplementedException();
        }

        public OrderLine GetOrderLineById(Guid id)
        {
            throw new NotImplementedException();
        }

        public void UpdateOrderLine(OrderLine orderLine)
        {
            throw new NotImplementedException();
        }
    }
}
