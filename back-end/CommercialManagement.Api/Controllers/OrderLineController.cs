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

        public OrderLineController(IOrderLineRepository orderLineRepository)
        {
            _orderLineRepository = orderLineRepository;
        }

        // GET: api/orderline
        [HttpGet]
        public ActionResult<IEnumerable<OderLineDto>> GetOrderLines()
        {
            var orderLines = _orderLineRepository.GetOrderLine();

            var dtos = orderLines.Select(ol => new OderLineDto
            {
                Id = ol.Id,
                Quantity = ol.Quantity,
                UnitPrice = ol.UnitPrice,
                TotalLine = ol.TotalLine,

                // Order Info
                OrderId = ol.OrderId,
                OrderNumber = ol.Order?.OrderNumber,
                OrderDate = ol.Order?.OrderDate ?? DateTime.UtcNow,
                Status = ol.Order?.Status ?? OrderStatus.Draft,
                TotalHT = ol.Order?.TotalHT ?? 0,
                TotalTTC = ol.Order?.TotalTTC ?? 0,

                // Client Info
                Client_Id = ol.Order?.ClientId ?? Guid.Empty,
                Client_LastName = ol.Order?.Client?.LastName,
                Client_FirstName = ol.Order?.Client?.FirstName,
                Client_Email = ol.Order?.Client?.Email,
                Client_Phone = ol.Order?.Client?.Phone,
                Client_Created = ol.Order?.Client?.Created ?? DateTime.UtcNow,

                // Address Info
                Adresse_Rue = ol.Order?.Client?.Adresse?.Rue,
                Adresse_Ville = ol.Order?.Client?.Adresse?.Ville,
                Adresse_CodePostal = ol.Order?.Client?.Adresse?.CodePostal,
                Adresse_Pays = ol.Order?.Client?.Adresse?.Pays,

                // Product Info
                Product_Id = ol.ProductId,
                Product_Reference = ol.Product?.Reference,
                Product_Name = ol.Product?.Name,
                Product_Description = ol.Product?.Description,
                Product_UnitPriceHT = ol.Product?.UnitPriceHT ?? 0,
                Product_StockQuantity = ol.Product?.StockQuantity ?? 0,
                Product_Created = ol.Product?.Created ?? DateTime.UtcNow
            }).ToList();

            return Ok(dtos);
        }

        // GET: api/orderline/{id}
        [HttpGet("{id}")]
        public ActionResult<OderLineDto> GetOrderLineById(Guid id)
        {
            var orderLine = _orderLineRepository.GetOrderLineById(id);

            if (orderLine == null)
                return NotFound();

            var dto = new OderLineDto
            {
                Id = orderLine.Id,
                Quantity = orderLine.Quantity,
                UnitPrice = orderLine.UnitPrice,
                TotalLine = orderLine.TotalLine,

                OrderId = orderLine.OrderId,
                OrderNumber = orderLine.Order?.OrderNumber,
                OrderDate = orderLine.Order?.OrderDate ?? DateTime.UtcNow,
                Status = orderLine.Order?.Status ?? OrderStatus.Draft,
                TotalHT = orderLine.Order?.TotalHT ?? 0,
                TotalTTC = orderLine.Order?.TotalTTC ?? 0,

                Client_Id = orderLine.Order?.ClientId ?? Guid.Empty,
                Client_LastName = orderLine.Order?.Client?.LastName,
                Client_FirstName = orderLine.Order?.Client?.FirstName,
                Client_Email = orderLine.Order?.Client?.Email,
                Client_Phone = orderLine.Order?.Client?.Phone,
                Client_Created = orderLine.Order?.Client?.Created ?? DateTime.UtcNow,

                Adresse_Rue = orderLine.Order?.Client?.Adresse?.Rue,
                Adresse_Ville = orderLine.Order?.Client?.Adresse?.Ville,
                Adresse_CodePostal = orderLine.Order?.Client?.Adresse?.CodePostal,
                Adresse_Pays = orderLine.Order?.Client?.Adresse?.Pays,

                Product_Id = orderLine.ProductId,
                Product_Reference = orderLine.Product?.Reference,
                Product_Name = orderLine.Product?.Name,
                Product_Description = orderLine.Product?.Description,
                Product_UnitPriceHT = orderLine.Product?.UnitPriceHT ?? 0,
                Product_StockQuantity = orderLine.Product?.StockQuantity ?? 0,
                Product_Created = orderLine.Product?.Created ?? DateTime.UtcNow
            };

            return Ok(dto);
        }

        // POST: api/orderline
        [HttpPost]
        public ActionResult<OrderLine> CreateOrderLine([FromBody] OrderLine orderLineInput)
        {
            if (orderLineInput == null)
                return BadRequest("OrderLine cannot be null");

            if (orderLineInput.OrderId == Guid.Empty || orderLineInput.ProductId == Guid.Empty)
                return BadRequest("OrderId and ProductId are required");

            var orderLineEntity = new OrderLine
            {
                Id = Guid.NewGuid(),
                OrderId = orderLineInput.OrderId,
                ProductId = orderLineInput.ProductId,
                Quantity = orderLineInput.Quantity,
                UnitPrice = orderLineInput.UnitPrice,
                TotalLine = orderLineInput.Quantity * orderLineInput.UnitPrice
            };

            _orderLineRepository.AddOrderLine(orderLineEntity);

            return CreatedAtAction(nameof(GetOrderLineById), new { id = orderLineEntity.Id }, orderLineEntity);
        }

        // PUT: api/orderline/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateOrderLine(Guid id, [FromBody] OrderLine orderLine)
        {
            if (orderLine == null)
                return BadRequest("OrderLine cannot be null");

            var existing = _orderLineRepository.GetOrderLineById(id);
            if (existing == null)
                return NotFound();

            existing.OrderId = orderLine.OrderId;
            existing.ProductId = orderLine.ProductId;
            existing.Quantity = orderLine.Quantity;
            existing.UnitPrice = orderLine.UnitPrice;
            existing.TotalLine = orderLine.Quantity * orderLine.UnitPrice;

            _orderLineRepository.UpdateOrderLine(existing);

            return NoContent();
        }

        // DELETE: api/orderline/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteOrderLine(Guid id)
        {
            var orderLine = _orderLineRepository.GetOrderLineById(id);
            if (orderLine == null)
                return NotFound();

            _orderLineRepository.DeleteOrderLine(orderLine);
            return NoContent();
        }
    }
}