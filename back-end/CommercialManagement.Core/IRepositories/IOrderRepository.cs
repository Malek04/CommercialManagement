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
        IEnumerable<Client> GetOrder();
        Client GetOrderById(Guid id);
        void AddOrder(Client client);
        void UpdateOrder(Client client);
        void DeleteOrder(Client client);
    }
}
