using CommercialManagement.Core.IRepositories;
using CommercialManagement.Core.Models;
using CommercialManagement.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace CommercialManagement.Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly CommercialManagementDbContext _context;

        public ClientRepository(CommercialManagementDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Client> GetClients()
        {
            return _context.Clients
                           .AsNoTracking()
                           .OrderByDescending(c => c.Created)
                           .ToList();
        }

        public Client? GetClientById(Guid id)
        {
            return _context.Clients
                           .AsNoTracking()
                           .FirstOrDefault(c => c.Id == id);
        }

        public void AddClient(Client client)
        {
            client.Id = Guid.NewGuid();
            client.Created = DateTime.UtcNow;

            _context.Clients.Add(client);
            _context.SaveChanges();
        }

        public void UpdateClient(Client client)
        {
            _context.Clients.Update(client);
            _context.SaveChanges();
        }

        public void DeleteClient(Client client)
        {
            _context.Clients.Remove(client);
            _context.SaveChanges();
        }
    }
}