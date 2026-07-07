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
        IEnumerable<Client> GetOrderLine();
        Client GetOrderLineById(Guid id);
        void AddOrderLine(Client client);
        void UpdateOrderLine(Client client);
        void DeleteOrderLine(Client client);
    }
}
