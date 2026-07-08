using CommercialManagement.Core.DTOs;
using CommercialManagement.Core.Enums;
using CommercialManagement.Core.IRepositories;
using CommercialManagement.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommercialManagement.Api.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderLineRepository _orderLineRepository;
        private readonly IProductRepository _productRepository;

        public OrderController(
            IOrderRepository orderRepository,
            IOrderLineRepository orderLineRepository,
            IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _orderLineRepository = orderLineRepository;
            _productRepository = productRepository;
        }

        // GET: api/orders
        [HttpGet]
        public ActionResult<IEnumerable<OrderDto>> GetOrders()
        {
            var orders = _orderRepository.GetOrder();
            return Ok(orders.Select(MapToDto));
        }

        // GET: api/orders/{id}
        [HttpGet("{id}")]
        public ActionResult<OrderDto> GetOrderById(Guid id)
        {
            var order = _orderRepository.GetOrderById(id);
            if (order == null)
                return NotFound(new { Message = $"Commande avec l'ID {id} non trouvée." });

            return Ok(MapToDto(order));
        }

        // POST: api/orders
        [HttpPost]
        public ActionResult<OrderDto> CreateOrder([FromBody] OrderRequestDto input)
        {
            if (input == null)
                return BadRequest(new { Message = "Les données de la commande sont obligatoires." });

            if (input.ClientId == Guid.Empty)
                return BadRequest(new { Message = "Un client est obligatoire pour créer une commande." });

            // Validation du stock AVANT toute écriture
            foreach (var line in input.Lines)
            {
                if (line.Quantity <= 0)
                    return BadRequest(new { Message = "La quantité doit être supérieure à 0." });

                var product = _productRepository.GetProductById(line.ProductId);
                if (product == null)
                    return NotFound(new { Message = $"Produit {line.ProductId} non trouvé." });

                if (line.Quantity > product.StockQuantity)
                    return BadRequest(new { Message = $"Stock insuffisant pour '{product.Name}'. Disponible : {product.StockQuantity}" });
            }

            var orderEntity = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = $"CMD-{DateTime.UtcNow:yyyyMMddHHmmss}",
                ClientId = input.ClientId,
                OrderDate = input.OrderDate == default ? DateTime.UtcNow : input.OrderDate,
                Status = OrderStatus.Draft
            };
            _orderRepository.AddOrder(orderEntity);

            foreach (var line in input.Lines)
            {
                var product = _productRepository.GetProductById(line.ProductId)!;
                _orderLineRepository.AddOrderLine(new OrderLine
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderEntity.Id,
                    ProductId = line.ProductId,
                    Quantity = line.Quantity,
                    UnitPrice = product.UnitPriceHT // toujours depuis le produit
                });
            }

            var saved = _orderRepository.GetOrderById(orderEntity.Id)!;
            saved.CalculateTotals();
            _orderRepository.UpdateOrder(saved);

            return CreatedAtAction(nameof(GetOrderById), new { id = saved.Id }, MapToDto(saved));
        }

        // PUT: api/orders/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateOrder(Guid id, [FromBody] OrderRequestDto input)
        {
            if (input == null)
                return BadRequest(new { Message = "Les données de la commande sont obligatoires." });

            if (input.ClientId == Guid.Empty)
                return BadRequest(new { Message = "Un client est obligatoire." });

            var existingOrder = _orderRepository.GetOrderById(id);
            if (existingOrder == null)
                return NotFound(new { Message = $"Commande avec l'ID {id} non trouvée." });

            if (existingOrder.Status == OrderStatus.Validated)
                return Conflict(new { Message = "Impossible de modifier une commande déjà validée." });

            // Validation AVANT toute écriture
            foreach (var line in input.Lines)
            {
                if (line.Quantity <= 0)
                    return BadRequest(new { Message = "La quantité doit être supérieure à 0." });

                var product = _productRepository.GetProductById(line.ProductId);
                if (product == null)
                    return NotFound(new { Message = $"Produit {line.ProductId} non trouvé." });

                if (line.Quantity > product.StockQuantity)
                    return BadRequest(new { Message = $"Stock insuffisant pour '{product.Name}'. Disponible : {product.StockQuantity}" });
            }

            existingOrder.ClientId = input.ClientId;
            existingOrder.OrderDate = input.OrderDate == default ? existingOrder.OrderDate : input.OrderDate;
            _orderRepository.UpdateOrder(existingOrder);

            // Synchronisation des lignes : update existantes, ajout des nouvelles, suppression des absentes
            var incomingIds = input.Lines.Where(l => l.Id.HasValue).Select(l => l.Id!.Value).ToHashSet();

            foreach (var existingLine in existingOrder.OrderLines.ToList())
            {
                if (!incomingIds.Contains(existingLine.Id))
                    _orderLineRepository.DeleteOrderLine(existingLine);
            }

            foreach (var line in input.Lines)
            {
                var product = _productRepository.GetProductById(line.ProductId)!;

                if (line.Id.HasValue)
                {
                    var existingLine = existingOrder.OrderLines.FirstOrDefault(l => l.Id == line.Id.Value);
                    if (existingLine == null) continue;

                    existingLine.Quantity = line.Quantity;
                    existingLine.UnitPrice = product.UnitPriceHT;
                    _orderLineRepository.UpdateOrderLine(existingLine);
                }
                else
                {
                    _orderLineRepository.AddOrderLine(new OrderLine
                    {
                        Id = Guid.NewGuid(),
                        OrderId = existingOrder.Id,
                        ProductId = line.ProductId,
                        Quantity = line.Quantity,
                        UnitPrice = product.UnitPriceHT
                    });
                }
            }

            var refreshed = _orderRepository.GetOrderById(id)!;
            refreshed.CalculateTotals();
            _orderRepository.UpdateOrder(refreshed);

            return NoContent();
        }

        // DELETE: api/orders/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(Guid id)
        {
            var order = _orderRepository.GetOrderById(id);
            if (order == null)
                return NotFound(new { Message = $"Commande avec l'ID {id} non trouvée." });

            if (order.Status == OrderStatus.Validated)
                return Conflict(new { Message = "Impossible de supprimer une commande déjà validée." });

            foreach (var line in order.OrderLines.ToList())
                _orderLineRepository.DeleteOrderLine(line);

            _orderRepository.DeleteOrder(order);
            return NoContent();
        }

        // POST: api/orders/{id}/validate
        [HttpPost("{id}/validate")]
        public IActionResult ValidateOrder(Guid id)
        {
            var order = _orderRepository.GetOrderById(id);
            if (order == null)
                return NotFound(new { Message = $"Commande avec l'ID {id} non trouvée." });

            if (order.Status == OrderStatus.Validated)
                return Conflict(new { Message = "Cette commande est déjà validée." });

            if (!order.OrderLines.Any())
                return BadRequest(new { Message = "Impossible de valider une commande vide." });

            foreach (var line in order.OrderLines)
            {
                if (line.Product == null)
                    return BadRequest(new { Message = "Produit introuvable sur une ligne." });

                if (line.Quantity > line.Product.StockQuantity)
                    return BadRequest(new { Message = $"Stock insuffisant pour le produit '{line.Product.Name}'. Disponible : {line.Product.StockQuantity}" });
            }

            foreach (var line in order.OrderLines)
                line.Product!.StockQuantity -= line.Quantity;

            order.Status = OrderStatus.Validated;
            order.CalculateTotals();
            _orderRepository.UpdateOrder(order);

            return Ok(new { message = "Commande validée avec succès et stock mis à jour." });
        }

        private OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalHT = order.TotalHT,
                TotalTTC = order.TotalTTC,

                ClientId = order.ClientId,
                LastName = order.Client?.LastName,
                FirstName = order.Client?.FirstName,
                Email = order.Client?.Email,
                Phone = order.Client?.Phone,
                Created = order.Client?.Created ?? DateTime.UtcNow,

                Rue = order.Client?.Adresse?.Rue,
                Ville = order.Client?.Adresse?.Ville,
                CodePostal = order.Client?.Adresse?.CodePostal,
                Pays = order.Client?.Adresse?.Pays,

                Lines = order.OrderLines.Select(ol => new OrderLineItemDto
                {
                    Id = ol.Id,
                    ProductId = ol.ProductId,
                    ProductReference = ol.Product?.Reference,
                    ProductName = ol.Product?.Name,
                    Quantity = ol.Quantity,
                    UnitPrice = ol.UnitPrice,
                    ProductStockQuantity = ol.Product?.StockQuantity ?? 0
                }).ToList()
            };
        }
    }
}