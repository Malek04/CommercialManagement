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
        public void AddOrderLine(Client client)
        {
            throw new NotImplementedException();
        }

        public void DeleteOrderLine(Client client)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Client> GetOrderLine()
        {
            throw new NotImplementedException();
        }

        public Client GetOrderLineById(Guid id)
        {
            throw new NotImplementedException();
        }

        public void UpdateOrderLine(Client client)
        {
            throw new NotImplementedException();
        }
    }
}
