using CommercialManagement.Core.IRepositories;
using CommercialManagement.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommercialManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;

        public ClientController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        // GET: api/clients
        [HttpGet]
        public ActionResult<IEnumerable<Client>> GetClients()
        {
            var clients = _clientRepository.GetClients();
            return Ok(clients);
        }

        // GET: api/clients/{id}
        [HttpGet("{id:guid}")]
        public ActionResult<Client> GetClient(Guid id)
        {
            var client = _clientRepository.GetClientById(id);
            if (client == null)
                return NotFound(new { Message = $"Client with id {id} not found." });

            return Ok(client);
        }

        // POST: api/clients
        [HttpPost]
        public IActionResult AddClient([FromBody] Client client)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(client.LastName))
                return BadRequest("Le nom est obligatoire.");

            _clientRepository.AddClient(client);

            return CreatedAtAction(
                nameof(GetClient),
                new { id = client.Id },
                client);
        }

        // PUT: api/clients/{id}
        [HttpPut("{id:guid}")]
        public IActionResult UpdateClient(Guid id, [FromBody] Client client)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != client.Id)
                return BadRequest("ID mismatch.");

            var existingClient = _clientRepository.GetClientById(id);
            if (existingClient == null)
                return NotFound(new { Message = $"Client with id {id} not found." });

            // Update scalar properties
            existingClient.LastName = client.LastName;
            existingClient.FirstName = client.FirstName;
            existingClient.Email = client.Email;
            existingClient.Phone = client.Phone;

            // Update Owned Entity (Adresse)
            existingClient.Adresse.Rue = client.Adresse.Rue;
            existingClient.Adresse.Ville = client.Adresse.Ville;
            existingClient.Adresse.CodePostal = client.Adresse.CodePostal;
            existingClient.Adresse.Pays = client.Adresse.Pays;

            _clientRepository.UpdateClient(existingClient);

            return Ok();
        }

        // DELETE: api/clients/{id}
        [HttpDelete("{id:guid}")]
        public IActionResult DeleteClient(Guid id)
        {
            var client = _clientRepository.GetClientById(id);
            if (client == null)
                return NotFound(new { Message = $"Client with id {id} not found." });

            _clientRepository.DeleteClient(client);

            return Ok();
        }
    }
}