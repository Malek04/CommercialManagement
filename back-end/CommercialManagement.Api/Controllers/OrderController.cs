using AutoMapper;
using CommercialManagement.Core.DTOs;
using CommercialManagement.Core.IRepositories;
using CommercialManagement.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommercialManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderController(
            IOrderRepository orderRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }


        // GET: api/order
        [HttpGet]
        public ActionResult<IEnumerable<OrderDto>> GetOrders()
        {
            var orders = _orderRepository.GetOrder();

            var ordersDto = _mapper.Map<IEnumerable<OrderDto>>(orders);

            return Ok(ordersDto);
        }


        // GET: api/order/{id}
        [HttpGet("{id}")]
        public ActionResult<OrderDto> GetOrderById(Guid id)
        {
            var order = _orderRepository.GetOrderById(id);

            if (order == null)
                return NotFound();

            var orderDto = _mapper.Map<OrderDto>(order);

            return Ok(orderDto);
        }


        // POST: api/order
        [HttpPost]
        public ActionResult<Order> CreateOrder([FromBody] Order orderInput)
        {
            if (orderInput == null)
                return BadRequest("Order cannot be null");

            var orderEntity = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = orderInput.OrderNumber,
                ClientId = orderInput.ClientId,
                OrderDate = orderInput.OrderDate == default ? DateTime.UtcNow : orderInput.OrderDate,
                Status = orderInput.Status,
                TotalHT = orderInput.TotalHT,
                TotalTTC = orderInput.TotalTTC
            };

            _orderRepository.AddOrder(orderEntity);

            // Retourne directement l'entité Order (pas de OrderDto)
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
                return BadRequest("Order cannot be null");

            var existingOrder = _orderRepository.GetOrderById(id);

            if (existingOrder == null)
                return NotFound();

            // Mise à jour des champs
            existingOrder.OrderNumber = order.OrderNumber;
            existingOrder.ClientId = order.ClientId;
            existingOrder.OrderDate = order.OrderDate;
            existingOrder.Status = order.Status;
            existingOrder.TotalHT = order.TotalHT;
            existingOrder.TotalTTC = order.TotalTTC;

            _orderRepository.UpdateOrder(existingOrder);

            return NoContent();
        }

        // DELETE: api/order/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(Guid id)
        {
            var order = _orderRepository.GetOrderById(id);

            if (order == null)
                return NotFound();

            _orderRepository.DeleteOrder(order);

            return NoContent();
        }
    }
}