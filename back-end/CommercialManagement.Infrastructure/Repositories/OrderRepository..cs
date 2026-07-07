using CommercialManagement.Core.IRepositories;
using CommercialManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommercialManagement.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        public void AddOrder(Client client)
        {
            throw new NotImplementedException();
        }

        public void DeleteOrder(Client client)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Client> GetOrder()
        {
            throw new NotImplementedException();
        }

        public Client GetOrderById(Guid id)
        {
            throw new NotImplementedException();
        }

        public void UpdateOrder(Client client)
        {
            throw new NotImplementedException();
        }
    }
}
