using CommercialManagement.Core.IRepositories;
using CommercialManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommercialManagement.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        public void AddProduct(Client client)
        {
            throw new NotImplementedException();
        }

        public void DeleteProduct(Client client)
        {
            throw new NotImplementedException();
        }

        public Client GetOProductById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Client> GetProduct()
        {
            throw new NotImplementedException();
        }

        public void UpdateProduct(Client client)
        {
            throw new NotImplementedException();
        }
    }
}
