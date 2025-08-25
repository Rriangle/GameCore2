using Microsoft.AspNetCore.Mvc;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;

namespace GameCore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerMarketController : ControllerBase
    {
        private readonly IPlayerMarketService _playerMarketService;

        public PlayerMarketController(IPlayerMarketService playerMarketService)
        {
            _playerMarketService = playerMarketService;
        }

        [HttpGet("listings")]
        public async Task<ActionResult<IEnumerable<PlayerMarketListingResponseDto>>> GetAllListings()
        {
            try
            {
                var listings = await _playerMarketService.GetAllListingsAsync();
                var response = listings.Select(l => new PlayerMarketListingResponseDto
                {
                    Id = l.Id,
                    SellerId = l.SellerId,
                    SellerName = l.Seller?.Username ?? string.Empty,
                    Title = l.Title,
                    Description = l.Description,
                    Price = l.Price,
                    Quantity = l.Quantity,
                    AvailableQuantity = l.AvailableQuantity,
                    Status = l.Status.ToString(),
                    ImageUrl = l.ImageUrl,
                    IsNegotiable = l.IsNegotiable,
                    CreatedAt = l.CreatedAt,
                    UpdatedAt = l.UpdatedAt,
                    ExpiresAt = l.ExpiresAt
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpGet("listings/category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<PlayerMarketListingResponseDto>>> GetListingsByCategory(int categoryId)
        {
            try
            {
                var listings = await _playerMarketService.GetListingsByCategoryAsync(categoryId);
                var response = listings.Select(l => new PlayerMarketListingResponseDto
                {
                    Id = l.Id,
                    SellerId = l.SellerId,
                    SellerName = l.Seller?.Username ?? string.Empty,
                    Title = l.Title,
                    Description = l.Description,
                    Price = l.Price,
                    Quantity = l.Quantity,
                    AvailableQuantity = l.AvailableQuantity,
                    Status = l.Status.ToString(),
                    ImageUrl = l.ImageUrl,
                    IsNegotiable = l.IsNegotiable,
                    CreatedAt = l.CreatedAt,
                    UpdatedAt = l.UpdatedAt,
                    ExpiresAt = l.ExpiresAt
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpGet("listings/user/{userId}")]
        public async Task<ActionResult<IEnumerable<PlayerMarketListingResponseDto>>> GetUserListings(int userId)
        {
            try
            {
                var listings = await _playerMarketService.GetUserListingsAsync(userId);
                var response = listings.Select(l => new PlayerMarketListingResponseDto
                {
                    Id = l.Id,
                    SellerId = l.SellerId,
                    SellerName = string.Empty, // Will be populated when fetched with include
                    Title = l.Title,
                    Description = l.Description,
                    Price = l.Price,
                    Quantity = l.Quantity,
                    AvailableQuantity = l.AvailableQuantity,
                    Status = l.Status.ToString(),
                    ImageUrl = l.ImageUrl,
                    IsNegotiable = l.IsNegotiable,
                    CreatedAt = l.CreatedAt,
                    UpdatedAt = l.UpdatedAt,
                    ExpiresAt = l.ExpiresAt
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpGet("listings/{id}")]
        public async Task<ActionResult<PlayerMarketListingResponseDto>> GetListingById(int id)
        {
            try
            {
                var listing = await _playerMarketService.GetListingByIdAsync(id);
                if (listing == null)
                    return NotFound(new { error = "Listing not found" });

                var response = new PlayerMarketListingResponseDto
                {
                    Id = listing.Id,
                    SellerId = listing.SellerId,
                    SellerName = listing.Seller?.Username ?? string.Empty,
                    Title = listing.Title,
                    Description = listing.Description,
                    Price = listing.Price,
                    Quantity = listing.Quantity,
                    AvailableQuantity = listing.AvailableQuantity,
                    Status = listing.Status.ToString(),
                    ImageUrl = listing.ImageUrl,
                    IsNegotiable = listing.IsNegotiable,
                    CreatedAt = listing.CreatedAt,
                    UpdatedAt = listing.UpdatedAt,
                    ExpiresAt = listing.ExpiresAt
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpPost("listings")]
        public async Task<ActionResult<PlayerMarketListingResponseDto>> CreateListing([FromBody] CreatePlayerMarketListingDto createListingDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var listing = await _playerMarketService.CreateListingAsync(createListingDto);
                var response = new PlayerMarketListingResponseDto
                {
                    Id = listing.Id,
                    SellerId = listing.SellerId,
                    SellerName = string.Empty, // Will be populated when fetched with include
                    Title = listing.Title,
                    Description = listing.Description,
                    Price = listing.Price,
                    Quantity = listing.Quantity,
                    AvailableQuantity = listing.AvailableQuantity,
                    Status = listing.Status.ToString(),
                    ImageUrl = listing.ImageUrl,
                    IsNegotiable = listing.IsNegotiable,
                    CreatedAt = listing.CreatedAt,
                    UpdatedAt = listing.UpdatedAt,
                    ExpiresAt = listing.ExpiresAt
                };

                return CreatedAtAction(nameof(GetListingById), new { id = listing.Id }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpPut("listings/{id}")]
        public async Task<ActionResult<PlayerMarketListingResponseDto>> UpdateListing(int id, [FromBody] UpdatePlayerMarketListingDto updateListingDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var listing = await _playerMarketService.UpdateListingAsync(id, updateListingDto);
                var response = new PlayerMarketListingResponseDto
                {
                    Id = listing.Id,
                    SellerId = listing.SellerId,
                    SellerName = string.Empty, // Will be populated when fetched with include
                    Title = listing.Title,
                    Description = listing.Description,
                    Price = listing.Price,
                    Quantity = listing.Quantity,
                    AvailableQuantity = listing.AvailableQuantity,
                    Status = listing.Status.ToString(),
                    ImageUrl = listing.ImageUrl,
                    IsNegotiable = listing.IsNegotiable,
                    CreatedAt = listing.CreatedAt,
                    UpdatedAt = listing.UpdatedAt,
                    ExpiresAt = listing.ExpiresAt
                };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = "Listing not found", message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpDelete("listings/{id}")]
        public async Task<ActionResult> DeleteListing(int id)
        {
            try
            {
                var result = await _playerMarketService.DeleteListingAsync(id);
                if (!result)
                    return NotFound(new { error = "Listing not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpPost("orders")]
        public async Task<ActionResult<PlayerMarketOrderResponseDto>> CreateOrder([FromBody] CreatePlayerMarketOrderDto createOrderDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var order = await _playerMarketService.CreateOrderAsync(createOrderDto);
                var response = new PlayerMarketOrderResponseDto
                {
                    Id = order.Id,
                    ListingId = order.ListingId,
                    ListingTitle = string.Empty, // Will be populated when fetched with include
                    BuyerId = order.BuyerId,
                    BuyerName = string.Empty, // Will be populated when fetched with include
                    SellerId = order.SellerId,
                    SellerName = string.Empty, // Will be populated when fetched with include
                    Quantity = order.Quantity,
                    UnitPrice = order.UnitPrice,
                    TotalAmount = order.TotalAmount,
                    PlatformFee = order.PlatformFee,
                    SellerAmount = order.SellerAmount,
                    Status = order.Status.ToString(),
                    BuyerNotes = order.BuyerNotes,
                    SellerNotes = order.SellerNotes,
                    CreatedAt = order.CreatedAt,
                    ConfirmedAt = order.ConfirmedAt,
                    CompletedAt = order.CompletedAt,
                    CancelledAt = order.CancelledAt
                };

                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = "Invalid request", message = ex.Message });
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

        [HttpGet("orders/{id}")]
        public async Task<ActionResult<PlayerMarketOrderResponseDto>> GetOrderById(int id)
        {
            try
            {
                // This would need to be implemented in the service
                // For now, return a placeholder
                return NotFound(new { error = "Order not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpPost("orders/{id}/confirm")]
        public async Task<ActionResult<PlayerMarketOrderResponseDto>> ConfirmOrder(int id)
        {
            try
            {
                var order = await _playerMarketService.ConfirmOrderAsync(id);
                var response = new PlayerMarketOrderResponseDto
                {
                    Id = order.Id,
                    ListingId = order.ListingId,
                    ListingTitle = string.Empty, // Will be populated when fetched with include
                    BuyerId = order.BuyerId,
                    BuyerName = string.Empty, // Will be populated when fetched with include
                    SellerId = order.SellerId,
                    SellerName = string.Empty, // Will be populated when fetched with include
                    Quantity = order.Quantity,
                    UnitPrice = order.UnitPrice,
                    TotalAmount = order.TotalAmount,
                    PlatformFee = order.PlatformFee,
                    SellerAmount = order.SellerAmount,
                    Status = order.Status.ToString(),
                    BuyerNotes = order.BuyerNotes,
                    SellerNotes = order.SellerNotes,
                    CreatedAt = order.CreatedAt,
                    ConfirmedAt = order.ConfirmedAt,
                    CompletedAt = order.CompletedAt,
                    CancelledAt = order.CancelledAt
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

        [HttpPost("orders/{id}/complete")]
        public async Task<ActionResult<PlayerMarketOrderResponseDto>> CompleteOrder(int id)
        {
            try
            {
                var order = await _playerMarketService.CompleteOrderAsync(id);
                var response = new PlayerMarketOrderResponseDto
                {
                    Id = order.Id,
                    ListingId = order.ListingId,
                    ListingTitle = string.Empty, // Will be populated when fetched with include
                    BuyerId = order.BuyerId,
                    BuyerName = string.Empty, // Will be populated when fetched with include
                    SellerId = order.SellerId,
                    SellerName = string.Empty, // Will be populated when fetched with include
                    Quantity = order.Quantity,
                    UnitPrice = order.UnitPrice,
                    TotalAmount = order.TotalAmount,
                    PlatformFee = order.PlatformFee,
                    SellerAmount = order.SellerAmount,
                    Status = order.Status.ToString(),
                    BuyerNotes = order.BuyerNotes,
                    SellerNotes = order.SellerNotes,
                    CreatedAt = order.CreatedAt,
                    ConfirmedAt = order.ConfirmedAt,
                    CompletedAt = order.CompletedAt,
                    CancelledAt = order.CancelledAt
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

        [HttpPost("orders/{id}/cancel")]
        public async Task<ActionResult> CancelOrder(int id)
        {
            try
            {
                var result = await _playerMarketService.CancelOrderAsync(id);
                if (!result)
                    return NotFound(new { error = "Order not found or cannot be cancelled" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpGet("orders/user/{userId}")]
        public async Task<ActionResult<IEnumerable<PlayerMarketOrderResponseDto>>> GetUserOrders(int userId)
        {
            try
            {
                var orders = await _playerMarketService.GetUserOrdersAsync(userId);
                var response = orders.Select(o => new PlayerMarketOrderResponseDto
                {
                    Id = o.Id,
                    ListingId = o.ListingId,
                    ListingTitle = o.Listing?.Title ?? string.Empty,
                    BuyerId = o.BuyerId,
                    BuyerName = o.Buyer?.Username ?? string.Empty,
                    SellerId = o.SellerId,
                    SellerName = o.Seller?.Username ?? string.Empty,
                    Quantity = o.Quantity,
                    UnitPrice = o.UnitPrice,
                    TotalAmount = o.TotalAmount,
                    PlatformFee = o.PlatformFee,
                    SellerAmount = o.SellerAmount,
                    Status = o.Status.ToString(),
                    BuyerNotes = o.BuyerNotes,
                    SellerNotes = o.SellerNotes,
                    CreatedAt = o.CreatedAt,
                    ConfirmedAt = o.ConfirmedAt,
                    CompletedAt = o.CompletedAt,
                    CancelledAt = o.CancelledAt
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpGet("sales/user/{userId}")]
        public async Task<ActionResult<IEnumerable<PlayerMarketOrderResponseDto>>> GetUserSales(int userId)
        {
            try
            {
                var sales = await _playerMarketService.GetUserSalesAsync(userId);
                var response = sales.Select(o => new PlayerMarketOrderResponseDto
                {
                    Id = o.Id,
                    ListingId = o.ListingId,
                    ListingTitle = o.Listing?.Title ?? string.Empty,
                    BuyerId = o.BuyerId,
                    BuyerName = o.Buyer?.Username ?? string.Empty,
                    SellerId = o.SellerId,
                    SellerName = o.Seller?.Username ?? string.Empty,
                    Quantity = o.Quantity,
                    UnitPrice = o.UnitPrice,
                    TotalAmount = o.TotalAmount,
                    PlatformFee = o.PlatformFee,
                    SellerAmount = o.SellerAmount,
                    Status = o.Status.ToString(),
                    BuyerNotes = o.BuyerNotes,
                    SellerNotes = o.SellerNotes,
                    CreatedAt = o.CreatedAt,
                    ConfirmedAt = o.ConfirmedAt,
                    CompletedAt = o.CompletedAt,
                    CancelledAt = o.CancelledAt
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