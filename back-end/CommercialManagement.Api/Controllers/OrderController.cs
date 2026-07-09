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
        private readonly IClientRepository _clientRepository;

        public OrderController(
            IOrderRepository orderRepository,
            IOrderLineRepository orderLineRepository,
            IProductRepository productRepository,
            IClientRepository clientRepository)
        {
            _orderRepository = orderRepository;
            _orderLineRepository = orderLineRepository;
            _productRepository = productRepository;
            _clientRepository = clientRepository;
        }


        [HttpGet]
        public ActionResult<IEnumerable<OrderDto>> GetOrders()
        {
            return Ok(_orderRepository.GetOrder().Select(MapToDto));
        }

        [HttpGet("{id}")]
        public ActionResult<OrderDto> GetOrderById(Guid id)
        {
            var order = _orderRepository.GetOrderById(id);
            if (order == null)
                return NotFound("Commande introuvable.");
            return Ok(MapToDto(order));
        }

        [HttpPost]
        public ActionResult<OrderDto> CreateOrder(OrderRequestDto input)
        {
            if (input.ClientId == Guid.Empty)
                return BadRequest("Client obligatoire.");

            var client = _clientRepository.GetClientById(input.ClientId);
            if (client == null)
                return NotFound("Client introuvable.");
            if (input.Lines == null || !input.Lines.Any())
                return BadRequest("Commande vide.");
            foreach (var line in input.Lines)
            {
                if (line.Quantity <= 0)
                    return BadRequest("Quantité invalide.");

                var product = _productRepository.GetProductById(line.ProductId);

                if (product == null)
                    return NotFound("Produit introuvable.");

                if (line.Quantity > product.StockQuantity)
                    return BadRequest($"Stock insuffisant pour {product.Name}.");
            }


            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = $"CMD-{DateTime.Now:yyyyMMddHHmmss}",
                ClientId = input.ClientId,
                OrderDate = input.OrderDate != default ? input.OrderDate : DateTime.Now,
                Status = OrderStatus.Draft
            };


            _orderRepository.AddOrder(order);


            foreach (var line in input.Lines)
            {
                var product = _productRepository.GetProductById(line.ProductId);


                _orderLineRepository.AddOrderLine(new OrderLine
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = product.Id,
                    Quantity = line.Quantity,
                    UnitPrice = product.UnitPriceHT,
                    TotalLine = line.Quantity * product.UnitPriceHT
                });
            }


            order.CalculateTotals();

            _orderRepository.UpdateOrder(order);


            return CreatedAtAction(
                nameof(GetOrderById),
                new { id = order.Id },
                MapToDto(order)
            );
        }



        [HttpPost("{id}/validate")]
        public IActionResult ValidateOrder(Guid id)
        {
            var order = _orderRepository.GetOrderById(id);


            if (order == null)
                return NotFound("Commande introuvable.");


            if (order.Status != OrderStatus.Draft)
                return BadRequest("Seules les commandes en brouillon peuvent être validées.");


            foreach (var line in order.OrderLines)
            {
                var product = _productRepository.GetProductById(line.ProductId);


                if (product == null)
                    return NotFound("Produit introuvable.");


                if (product.StockQuantity < line.Quantity)
                    return BadRequest($"Stock insuffisant pour {product.Name}.");


                product.StockQuantity -= line.Quantity;

                _productRepository.UpdateProduct(product);
            }


            order.Status = OrderStatus.Validated;

            order.CalculateTotals();

            _orderRepository.UpdateOrder(order);


            return Ok();
        }

        [HttpPost("{id}/cancel")]
        public IActionResult CancelOrder(Guid id)
        {
            var order = _orderRepository.GetOrderById(id);


            if (order == null)
                return NotFound("Commande introuvable.");


            if (order.Status != OrderStatus.Draft)
                return BadRequest("Seules les commandes en brouillon peuvent être annulées.");


            order.Status = OrderStatus.Cancelled;


            _orderRepository.UpdateOrder(order);


            return Ok();
        }


        [HttpPut("{id}")]
        public IActionResult UpdateOrder(Guid id, OrderRequestDto input)
        {
            var order = _orderRepository.GetOrderById(id);

            if (order == null)
                return NotFound("Commande introuvable.");

            if (order.Status != OrderStatus.Draft)
                return BadRequest("Seules les commandes en brouillon peuvent être modifiées.");

            if (input.ClientId == Guid.Empty)
                return BadRequest("Client obligatoire.");

            var client = _clientRepository.GetClientById(input.ClientId);
            if (client == null)
                return NotFound("Client introuvable.");

            if (input.Lines == null || !input.Lines.Any())
                return BadRequest("Commande vide.");

            var resolvedProducts = new List<(OrderLineRequestDto Line, Product Product)>();
            foreach (var line in input.Lines)
            {
                if (line.Quantity <= 0)
                    return BadRequest("Quantité invalide.");

                var product = _productRepository.GetProductById(line.ProductId);
                if (product == null)
                    return NotFound("Produit introuvable.");

                if (line.Quantity > product.StockQuantity)
                    return BadRequest($"Stock insuffisant pour {product.Name}.");

                resolvedProducts.Add((line, product));
            }

            foreach (var oldLine in order.OrderLines.ToList())
            {
                _orderLineRepository.DeleteOrderLine(oldLine);
            }

            order.ClientId = input.ClientId;
            if (input.OrderDate != default)
            {
                order.OrderDate = input.OrderDate;
            }

            foreach (var (line, product) in resolvedProducts)
            {
                _orderLineRepository.AddOrderLine(new OrderLine
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = product.Id,
                    Quantity = line.Quantity,
                    UnitPrice = product.UnitPriceHT,
                    TotalLine = line.Quantity * product.UnitPriceHT
                });
            }

            order.CalculateTotals();
            _orderRepository.UpdateOrder(order);

            return NoContent();
        }




        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(Guid id)
        {
            var order = _orderRepository.GetOrderById(id);


            if (order == null)
                return NotFound("Commande introuvable.");


            if (order.Status != OrderStatus.Draft)
                return BadRequest("Seules les commandes en brouillon peuvent être supprimées.");


            foreach (var line in order.OrderLines)
            {
                _orderLineRepository.DeleteOrderLine(line);
            }


            _orderRepository.DeleteOrder(order);


            return NoContent();
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

                FirstName = order.Client?.FirstName,
                LastName = order.Client?.LastName,
                Email = order.Client?.Email,
                Phone = order.Client?.Phone,
                Rue = order.Client?.Adresse.Rue,
                CodePostal = order.Client?.Adresse.CodePostal,
                Ville = order.Client?.Adresse.Ville,
                Pays = order.Client?.Adresse.Pays,


                Lines = order.OrderLines.Select(x => new OrderLineItemDto
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    ProductName = x.Product?.Name,
                    ProductReference = x.Product?.Reference,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice

                }).ToList()
            };
        }
    }
}