using Microsoft.AspNetCore.Mvc;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using GameCore.Domain.Entities;

namespace GameCore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetUserOrders(int userId)
        {
            try
            {
                var orders = await _orderService.GetUserOrdersAsync(userId);
                var response = orders.Select(o => new OrderResponseDto
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    UserId = o.UserId,
                    UserName = o.User?.Username ?? string.Empty,
                    TotalAmount = o.TotalAmount,
                    TaxAmount = o.TaxAmount,
                    ShippingAmount = o.ShippingAmount,
                    FinalAmount = o.FinalAmount,
                    Status = o.Status,
                    PaymentStatus = o.PaymentStatus,
                    ShippingAddress = o.ShippingAddress,
                    ShippingCity = o.ShippingCity,
                    ShippingPostalCode = o.ShippingPostalCode,
                    ShippingCountry = o.ShippingCountry,
                    ContactPhone = o.ContactPhone,
                    Notes = o.Notes,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt,
                    PaidAt = o.PaidAt,
                    ShippedAt = o.ShippedAt,
                    DeliveredAt = o.DeliveredAt,
                    CancelledAt = o.CancelledAt,
                    OrderItems = o.OrderItems.Select(oi => new OrderItemResponseDto
                    {
                        Id = oi.Id,
                        ProductId = oi.ProductId,
                        ProductName = oi.ProductName,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        TotalPrice = oi.TotalPrice,
                        ProductImageUrl = oi.ProductImageUrl
                    }).ToList()
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponseDto>> GetOrderById(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound(new { error = "Order not found" });

                var response = new OrderResponseDto
                {
                    Id = order.Id,
                    OrderNumber = order.OrderNumber,
                    UserId = order.UserId,
                    UserName = order.User?.Username ?? string.Empty,
                    TotalAmount = order.TotalAmount,
                    TaxAmount = order.TaxAmount,
                    ShippingAmount = order.ShippingAmount,
                    FinalAmount = order.FinalAmount,
                    Status = order.Status,
                    PaymentStatus = order.PaymentStatus,
                    ShippingAddress = order.ShippingAddress,
                    ShippingCity = order.ShippingCity,
                    ShippingPostalCode = order.ShippingPostalCode,
                    ShippingCountry = order.ShippingCountry,
                    ContactPhone = order.ContactPhone,
                    Notes = order.Notes,
                    CreatedAt = order.CreatedAt,
                    UpdatedAt = order.UpdatedAt,
                    PaidAt = order.PaidAt,
                    ShippedAt = order.ShippedAt,
                    DeliveredAt = order.DeliveredAt,
                    CancelledAt = order.CancelledAt,
                    OrderItems = order.OrderItems.Select(oi => new OrderItemResponseDto
                    {
                        Id = oi.Id,
                        ProductId = oi.ProductId,
                        ProductName = oi.ProductName,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        TotalPrice = oi.TotalPrice,
                        ProductImageUrl = oi.ProductImageUrl
                    }).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<OrderResponseDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var order = await _orderService.CreateOrderAsync(createOrderDto);
                var response = new OrderResponseDto
                {
                    Id = order.Id,
                    OrderNumber = order.OrderNumber,
                    UserId = order.UserId,
                    UserName = string.Empty, // Will be populated when fetched with include
                    TotalAmount = order.TotalAmount,
                    TaxAmount = order.TaxAmount,
                    ShippingAmount = order.ShippingAmount,
                    FinalAmount = order.FinalAmount,
                    Status = order.Status,
                    PaymentStatus = order.PaymentStatus,
                    ShippingAddress = order.ShippingAddress,
                    ShippingCity = order.ShippingCity,
                    ShippingPostalCode = order.ShippingPostalCode,
                    ShippingCountry = order.ShippingCountry,
                    ContactPhone = order.ContactPhone,
                    Notes = order.Notes,
                    CreatedAt = order.CreatedAt,
                    UpdatedAt = order.UpdatedAt,
                    PaidAt = order.PaidAt,
                    ShippedAt = order.ShippedAt,
                    DeliveredAt = order.DeliveredAt,
                    CancelledAt = order.CancelledAt,
                    OrderItems = order.OrderItems.Select(oi => new OrderItemResponseDto
                    {
                        Id = oi.Id,
                        ProductId = oi.ProductId,
                        ProductName = oi.ProductName,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        TotalPrice = oi.TotalPrice,
                        ProductImageUrl = oi.ProductImageUrl
                    }).ToList()
                };

                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = "Invalid request", message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult<OrderResponseDto>> UpdateOrderStatus(int id, [FromBody] OrderStatus status)
        {
            try
            {
                var order = await _orderService.UpdateOrderStatusAsync(id, status);
                var response = new OrderResponseDto
                {
                    Id = order.Id,
                    OrderNumber = order.OrderNumber,
                    UserId = order.UserId,
                    UserName = string.Empty, // Will be populated when fetched with include
                    TotalAmount = order.TotalAmount,
                    TaxAmount = order.TaxAmount,
                    ShippingAmount = order.ShippingAmount,
                    FinalAmount = order.FinalAmount,
                    Status = order.Status,
                    PaymentStatus = order.PaymentStatus,
                    ShippingAddress = order.ShippingAddress,
                    ShippingCity = order.ShippingCity,
                    ShippingPostalCode = order.ShippingPostalCode,
                    ShippingCountry = order.ShippingCountry,
                    ContactPhone = order.ContactPhone,
                    Notes = order.Notes,
                    CreatedAt = order.CreatedAt,
                    UpdatedAt = order.UpdatedAt,
                    PaidAt = order.PaidAt,
                    ShippedAt = order.ShippedAt,
                    DeliveredAt = order.DeliveredAt,
                    CancelledAt = order.CancelledAt,
                    OrderItems = new List<OrderItemResponseDto>() // Will be populated when fetched with include
                };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = "Order not found", message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<ActionResult> CancelOrder(int id)
        {
            try
            {
                var result = await _orderService.CancelOrderAsync(id);
                if (!result)
                    return NotFound(new { error = "Order not found or cannot be cancelled" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpPost("{id}/payment")]
        public async Task<ActionResult<OrderResponseDto>> ProcessPayment(int id, [FromBody] ProcessPaymentDto processPaymentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var order = await _orderService.ProcessPaymentAsync(id, processPaymentDto);
                var response = new OrderResponseDto
                {
                    Id = order.Id,
                    OrderNumber = order.OrderNumber,
                    UserId = order.UserId,
                    UserName = string.Empty, // Will be populated when fetched with include
                    TotalAmount = order.TotalAmount,
                    TaxAmount = order.TaxAmount,
                    ShippingAmount = order.ShippingAmount,
                    FinalAmount = order.FinalAmount,
                    Status = order.Status,
                    PaymentStatus = order.PaymentStatus,
                    ShippingAddress = order.ShippingAddress,
                    ShippingCity = order.ShippingCity,
                    ShippingPostalCode = order.ShippingPostalCode,
                    ShippingCountry = order.ShippingCountry,
                    ContactPhone = order.ContactPhone,
                    Notes = order.Notes,
                    CreatedAt = order.CreatedAt,
                    UpdatedAt = order.UpdatedAt,
                    PaidAt = order.PaidAt,
                    ShippedAt = order.ShippedAt,
                    DeliveredAt = order.DeliveredAt,
                    CancelledAt = order.CancelledAt,
                    OrderItems = new List<OrderItemResponseDto>() // Will be populated when fetched with include
                };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = "Order not found", message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = "Invalid operation", message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpPost("{id}/confirm")]
        public async Task<ActionResult> ConfirmOrder(int id)
        {
            try
            {
                var result = await _orderService.ConfirmOrderAsync(id);
                if (!result)
                    return BadRequest(new { error = "Order cannot be confirmed" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpPost("{id}/ship")]
        public async Task<ActionResult> ShipOrder(int id)
        {
            try
            {
                var result = await _orderService.ShipOrderAsync(id);
                if (!result)
                    return BadRequest(new { error = "Order cannot be shipped" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpPost("{id}/deliver")]
        public async Task<ActionResult> DeliverOrder(int id)
        {
            try
            {
                var result = await _orderService.DeliverOrderAsync(id);
                if (!result)
                    return BadRequest(new { error = "Order cannot be delivered" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetOrdersByStatus(OrderStatus status)
        {
            try
            {
                var orders = await _orderService.GetOrdersByStatusAsync(status);
                var response = orders.Select(o => new OrderResponseDto
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    UserId = o.UserId,
                    UserName = string.Empty, // Will be populated when fetched with include
                    TotalAmount = o.TotalAmount,
                    TaxAmount = o.TaxAmount,
                    ShippingAmount = o.ShippingAmount,
                    FinalAmount = o.FinalAmount,
                    Status = o.Status,
                    PaymentStatus = o.PaymentStatus,
                    ShippingAddress = o.ShippingAddress,
                    ShippingCity = o.ShippingCity,
                    ShippingPostalCode = o.ShippingPostalCode,
                    ShippingCountry = o.ShippingCountry,
                    ContactPhone = o.ContactPhone,
                    Notes = o.Notes,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt,
                    PaidAt = o.PaidAt,
                    ShippedAt = o.ShippedAt,
                    DeliveredAt = o.DeliveredAt,
                    CancelledAt = o.CancelledAt,
                    OrderItems = o.OrderItems.Select(oi => new OrderItemResponseDto
                    {
                        Id = oi.Id,
                        ProductId = oi.ProductId,
                        ProductName = oi.ProductName,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        TotalPrice = oi.TotalPrice,
                        ProductImageUrl = oi.ProductImageUrl
                    }).ToList()
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }
    }
}