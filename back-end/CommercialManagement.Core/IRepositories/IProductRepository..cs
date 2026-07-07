using CommercialManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommercialManagement.Core.IRepositories
{
    public interface IProductRepository
    {
        IEnumerable<Client> GetProduct();
        Client GetOProductById(Guid id);
        void AddProduct(Client client);
        void UpdateProduct(Client client);
        void DeleteProduct(Client client);
    }
}
