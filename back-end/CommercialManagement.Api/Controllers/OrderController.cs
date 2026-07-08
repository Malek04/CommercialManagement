using CommercialManagement.Core.Enums;
using CommercialManagement.Core.IRepositories;
using CommercialManagement.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommercialManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrderController(
            IOrderRepository orderRepository,
            IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        // GET: api/order
        [HttpGet]
        public ActionResult<IEnumerable<Order>> GetOrders()
        {
            var orders = _orderRepository.GetOrder();
            return Ok(orders);
        }

        // GET: api/order/{id}
        [HttpGet("{id}")]
        public ActionResult<Order> GetOrderById(Guid id)
        {
            var order = _orderRepository.GetOrderById(id);

            if (order == null)
                return NotFound(new { Message = $"Commande avec l'ID {id} non trouvée." });

            return Ok(order);
        }

        // POST: api/order
        [HttpPost]
        public ActionResult<Order> CreateOrder([FromBody] Order orderInput)
        {
            if (orderInput == null)
                return BadRequest(new { Message = "Les données de la commande sont obligatoires." });

            if (orderInput.ClientId == Guid.Empty)
                return BadRequest(new { Message = "Un client est obligatoire pour créer une commande." });

            var orderEntity = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = orderInput.OrderNumber ?? $"CMD-{DateTime.UtcNow:yyyyMMddHHmmss}",
                ClientId = orderInput.ClientId,
                OrderDate = orderInput.OrderDate == default ? DateTime.UtcNow : orderInput.OrderDate,
                Status = orderInput.Status == default ? OrderStatus.Draft : orderInput.Status
            };

            _orderRepository.AddOrder(orderEntity);

            return CreatedAtAction(
                nameof(GetOrderById),
                new { id = orderEntity.Id },
                orderEntity
            );
        }

        // PUT: api/order/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateOrder(Guid id, [FromBody] Order order)
        {
            if (order == null)
                return BadRequest(new { Message = "Les données de la commande sont obligatoires." });

            if (order.ClientId == Guid.Empty)
                return BadRequest(new { Message = "Un client est obligatoire." });

            var existingOrder = _orderRepository.GetOrderById(id);
            if (existingOrder == null)
                return NotFound(new { Message = $"Commande avec l'ID {id} non trouvée." });

            // On interdit la modification d'une commande déjà validée
            if (existingOrder.Status == OrderStatus.Validated)
                return BadRequest(new { Message = "Impossible de modifier une commande déjà validée." });

            existingOrder.OrderNumber = order.OrderNumber;
            existingOrder.ClientId = order.ClientId;
            existingOrder.OrderDate = order.OrderDate;
            existingOrder.Status = order.Status;

            existingOrder.CalculateTotals();

            _orderRepository.UpdateOrder(existingOrder);

            return NoContent();
        }

        // DELETE: api/order/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(Guid id)
        {
            var order = _orderRepository.GetOrderById(id);
            if (order == null)
                return NotFound(new { Message = $"Commande avec l'ID {id} non trouvée." });

            if (order.Status == OrderStatus.Validated)
                return BadRequest(new { Message = "Impossible de supprimer une commande déjà validée." });

            _orderRepository.DeleteOrder(order);
            return NoContent();
        }

        // POST: api/order/{id}/validate
        [HttpPost("{id}/validate")]
        public IActionResult ValidateOrder(Guid id)
        {
            var order = _orderRepository.GetOrderById(id);
            if (order == null)
                return NotFound(new { Message = $"Commande avec l'ID {id} non trouvée." });

            if (order.Status == OrderStatus.Validated)
                return BadRequest(new { Message = "Cette commande est déjà validée." });

            if (!order.OrderLines.Any())
                return BadRequest(new { Message = "Impossible de valider une commande vide." });

            // Vérification du stock pour toutes les lignes
            foreach (var line in order.OrderLines)
            {
                if (line.Product == null)
                    return BadRequest(new { Message = "Produit introuvable sur une ligne." });

                if (line.Quantity > line.Product.StockQuantity)
                    return BadRequest(new { Message = $"Stock insuffisant pour le produit '{line.Product.Name}'. Disponible : {line.Product.StockQuantity}" });
            }

            // Mise à jour du stock
            foreach (var line in order.OrderLines)
            {
                line.Product.StockQuantity -= line.Quantity;
            }

            order.Status = OrderStatus.Validated;
            order.CalculateTotals();

            _orderRepository.UpdateOrder(order);

            return Ok(new { message = "Commande validée avec succès et stock mis à jour." });
        }
    }
}