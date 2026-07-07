using CommercialManagement.Core.IRepositories;
using CommercialManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommercialManagement.Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        public void AddClient(Client client)
        {
            throw new NotImplementedException();
        }

        public void DeleteClient(Client client)
        {
            throw new NotImplementedException();
        }

        public Client GetClientById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Client> GetClients()
        {
            throw new NotImplementedException();
        }

        public void UpdateClient(Client client)
        {
            throw new NotImplementedException();
        }
    }
}
