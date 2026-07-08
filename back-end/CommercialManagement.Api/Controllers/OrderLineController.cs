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
    [Route("api/[controller]")]
    [ApiController]
    public class OrderLineController : ControllerBase
    {
        private readonly IOrderLineRepository _orderLineRepository;
        private readonly IProductRepository _productRepository;

        public OrderLineController(IOrderLineRepository orderLineRepository, IProductRepository productRepository)
        {
            _orderLineRepository = orderLineRepository;
            _productRepository = productRepository;
        }

        // GET: api/orderline
        [HttpGet]
        public ActionResult<IEnumerable<OrderLineDto>> GetOrderLines()
        {
            var orderLines = _orderLineRepository.GetOrderLine();
            var dtos = orderLines.Select(MapToDto).ToList();
            return Ok(dtos);
        }

        // GET: api/orderline/{id}
        [HttpGet("{id}")]
        public ActionResult<OrderLineDto> GetOrderLineById(Guid id)
        {
            var orderLine = _orderLineRepository.GetOrderLineById(id);
            if (orderLine == null)
                return NotFound(new { Message = $"Ligne de commande {id} non trouvée." });

            return Ok(MapToDto(orderLine));
        }

        // POST: api/orderline
        [HttpPost]
        public ActionResult<OrderLine> CreateOrderLine([FromBody] OrderLine orderLineInput)
        {
            if (orderLineInput == null)
                return BadRequest(new { Message = "Les données de la ligne sont obligatoires." });

            if (orderLineInput.OrderId == Guid.Empty || orderLineInput.ProductId == Guid.Empty)
                return BadRequest(new { Message = "OrderId et ProductId sont obligatoires." });

            if (orderLineInput.Quantity <= 0)
                return BadRequest(new { Message = "La quantité doit être supérieure à 0." });

            // Vérification stock
            var product = _productRepository.GetProductById(orderLineInput.ProductId);
            if (product == null)
                return NotFound(new { Message = "Produit non trouvé." });

            if (orderLineInput.Quantity > product.StockQuantity)
                return BadRequest(new { Message = $"Stock insuffisant. Seulement {product.StockQuantity} disponible." });

            var orderLineEntity = new OrderLine
            {
                Id = Guid.NewGuid(),
                OrderId = orderLineInput.OrderId,
                ProductId = orderLineInput.ProductId,
                Quantity = orderLineInput.Quantity,
                UnitPrice = orderLineInput.UnitPrice
            };

            _orderLineRepository.AddOrderLine(orderLineEntity);

            return CreatedAtAction(nameof(GetOrderLineById),
                new { id = orderLineEntity.Id }, orderLineEntity);
        }

        // PUT: api/orderline/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateOrderLine(Guid id, [FromBody] OrderLine orderLine)
        {
            if (orderLine == null)
                return BadRequest(new { Message = "Les données sont obligatoires." });

            var existing = _orderLineRepository.GetOrderLineById(id);
            if (existing == null)
                return NotFound(new { Message = $"Ligne de commande {id} non trouvée." });

            if (orderLine.Quantity <= 0)
                return BadRequest(new { Message = "La quantité doit être supérieure à 0." });

            // Vérification stock uniquement si quantité augmente
            if (orderLine.Quantity > existing.Quantity)
            {
                var product = _productRepository.GetProductById(orderLine.ProductId);
                if (product != null && orderLine.Quantity > product.StockQuantity)
                    return BadRequest(new { Message = $"Stock insuffisant. Seulement {product.StockQuantity} disponible." });
            }

            existing.OrderId = orderLine.OrderId;
            existing.ProductId = orderLine.ProductId;
            existing.Quantity = orderLine.Quantity;
            existing.UnitPrice = orderLine.UnitPrice;

            _orderLineRepository.UpdateOrderLine(existing);

            return NoContent();
        }

        // DELETE: api/orderline/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteOrderLine(Guid id)
        {
            var orderLine = _orderLineRepository.GetOrderLineById(id);
            if (orderLine == null)
                return NotFound(new { Message = $"Ligne de commande {id} non trouvée." });

            _orderLineRepository.DeleteOrderLine(orderLine);
            return NoContent();
        }

        // ====================== Méthode privée ======================
        private OrderLineDto MapToDto(OrderLine ol)
        {
            return new OrderLineDto
            {
                Id = ol.Id,
                Quantity = ol.Quantity,
                UnitPrice = ol.UnitPrice,
                TotalLine = ol.Quantity * ol.UnitPrice,

                OrderId = ol.OrderId,
                OrderNumber = ol.Order?.OrderNumber,
                OrderDate = ol.Order?.OrderDate ?? DateTime.UtcNow,
                Status = ol.Order?.Status ?? OrderStatus.Draft,
                TotalHT = ol.Order?.TotalHT ?? 0,
                TotalTTC = ol.Order?.TotalTTC ?? 0,

                Client_Id = ol.Order?.ClientId ?? Guid.Empty,
                Client_LastName = ol.Order?.Client?.LastName,
                Client_FirstName = ol.Order?.Client?.FirstName,
                Client_Email = ol.Order?.Client?.Email,
                Client_Phone = ol.Order?.Client?.Phone,
                Client_Created = ol.Order?.Client?.Created ?? DateTime.UtcNow,

                Adresse_Rue = ol.Order?.Client?.Adresse?.Rue,
                Adresse_Ville = ol.Order?.Client?.Adresse?.Ville,
                Adresse_CodePostal = ol.Order?.Client?.Adresse?.CodePostal,
                Adresse_Pays = ol.Order?.Client?.Adresse?.Pays,

                Product_Id = ol.ProductId,
                Product_Reference = ol.Product?.Reference,
                Product_Name = ol.Product?.Name,
                Product_Description = ol.Product?.Description,
                Product_UnitPriceHT = ol.Product?.UnitPriceHT ?? 0,
                Product_StockQuantity = ol.Product?.StockQuantity ?? 0,
                Product_Created = ol.Product?.Created ?? DateTime.UtcNow
            };
        }
    }
}