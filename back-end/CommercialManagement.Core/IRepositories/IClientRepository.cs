using CommercialManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommercialManagement.Core.IRepositories
{
    public interface IClientRepository
    {
        IEnumerable<Client> GetClients();
        Client GetClientById(Guid id);
        void AddClient(Client client);
        void UpdateClient(Client client);
        void DeleteClient(Client client);
    }
}
