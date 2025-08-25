using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using GameCore.Infrastructure.Data;

namespace GameCore.Infrastructure.Services
{
    public class PlayerMarketService : IPlayerMarketService
    {
        private readonly GameCoreDbContext _context;

        public PlayerMarketService(GameCoreDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PlayerMarketListing>> GetAllListingsAsync()
        {
            return await _context.PlayerMarketListings
                .Include(l => l.Seller)
                .Where(l => l.Status == ListingStatus.Active)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PlayerMarketListing>> GetListingsByCategoryAsync(int categoryId)
        {
            // Note: This would need to be implemented with a category system for player market
            // For now, return all active listings
            return await GetAllListingsAsync();
        }

        public async Task<IEnumerable<PlayerMarketListing>> GetUserListingsAsync(int userId)
        {
            return await _context.PlayerMarketListings
                .Where(l => l.SellerId == userId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<PlayerMarketListing?> GetListingByIdAsync(int id)
        {
            return await _context.PlayerMarketListings
                .Include(l => l.Seller)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<PlayerMarketListing> CreateListingAsync(CreatePlayerMarketListingDto createListingDto)
        {
            var listing = new PlayerMarketListing
            {
                SellerId = createListingDto.SellerId,
                Title = createListingDto.Title,
                Description = createListingDto.Description,
                Price = createListingDto.Price,
                Quantity = createListingDto.Quantity,
                AvailableQuantity = createListingDto.Quantity,
                Status = ListingStatus.Active,
                ImageUrl = createListingDto.ImageUrl,
                IsNegotiable = createListingDto.IsNegotiable,
                ExpiresAt = createListingDto.ExpiresAt ?? DateTime.UtcNow.AddDays(30), // Default 30 days
                CreatedAt = DateTime.UtcNow
            };

            _context.PlayerMarketListings.Add(listing);
            await _context.SaveChangesAsync();

            return listing;
        }

        public async Task<PlayerMarketListing> UpdateListingAsync(int id, UpdatePlayerMarketListingDto updateListingDto)
        {
            var listing = await _context.PlayerMarketListings.FindAsync(id);
            if (listing == null)
                throw new ArgumentException("Listing not found");

            if (updateListingDto.Title != null)
                listing.Title = updateListingDto.Title;
            if (updateListingDto.Description != null)
                listing.Description = updateListingDto.Description;
            if (updateListingDto.Price.HasValue)
                listing.Price = updateListingDto.Price.Value;
            if (updateListingDto.Quantity.HasValue)
            {
                var quantityChange = updateListingDto.Quantity.Value - listing.Quantity;
                listing.Quantity = updateListingDto.Quantity.Value;
                listing.AvailableQuantity = Math.Max(0, listing.AvailableQuantity + quantityChange);
            }
            if (updateListingDto.ImageUrl != null)
                listing.ImageUrl = updateListingDto.ImageUrl;
            if (updateListingDto.IsNegotiable.HasValue)
                listing.IsNegotiable = updateListingDto.IsNegotiable.Value;
            if (updateListingDto.ExpiresAt.HasValue)
                listing.ExpiresAt = updateListingDto.ExpiresAt.Value;

            listing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return listing;
        }

        public async Task<bool> DeleteListingAsync(int id)
        {
            var listing = await _context.PlayerMarketListings.FindAsync(id);
            if (listing == null)
                return false;

            listing.Status = ListingStatus.Cancelled;
            listing.CancelledAt = DateTime.UtcNow;
            listing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PlayerMarketOrder> CreateOrderAsync(CreatePlayerMarketOrderDto createOrderDto)
        {
            var listing = await _context.PlayerMarketListings.FindAsync(createOrderDto.ListingId);
            if (listing == null)
                throw new ArgumentException("Listing not found");

            if (listing.Status != ListingStatus.Active)
                throw new InvalidOperationException("Listing is not active");

            if (listing.AvailableQuantity < createOrderDto.Quantity)
                throw new InvalidOperationException("Insufficient available quantity");

            var seller = await _context.Users.FindAsync(listing.SellerId);
            if (seller == null)
                throw new ArgumentException("Seller not found");

            var buyer = await _context.Users.FindAsync(createOrderDto.BuyerId);
            if (buyer == null)
                throw new ArgumentException("Buyer not found");

            if (createOrderDto.BuyerId == listing.SellerId)
                throw new InvalidOperationException("Buyer cannot be the same as seller");

            var totalAmount = listing.Price * createOrderDto.Quantity;
            var platformFee = CalculatePlatformFeeAsync(totalAmount);
            var sellerAmount = totalAmount - platformFee;

            var order = new PlayerMarketOrder
            {
                ListingId = createOrderDto.ListingId,
                BuyerId = createOrderDto.BuyerId,
                SellerId = listing.SellerId,
                Quantity = createOrderDto.Quantity,
                UnitPrice = listing.Price,
                TotalAmount = totalAmount,
                PlatformFee = platformFee,
                SellerAmount = sellerAmount,
                Status = PlayerMarketOrderStatus.Pending,
                BuyerNotes = createOrderDto.BuyerNotes,
                CreatedAt = DateTime.UtcNow
            };

            _context.PlayerMarketOrders.Add(order);

            // Update listing available quantity
            listing.AvailableQuantity -= createOrderDto.Quantity;
            if (listing.AvailableQuantity == 0)
            {
                listing.Status = ListingStatus.Sold;
                listing.SoldAt = DateTime.UtcNow;
            }
            listing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<PlayerMarketOrder> ConfirmOrderAsync(int orderId)
        {
            var order = await _context.PlayerMarketOrders.FindAsync(orderId);
            if (order == null)
                throw new ArgumentException("Order not found");

            if (order.Status != PlayerMarketOrderStatus.Pending)
                throw new InvalidOperationException("Order cannot be confirmed in current status");

            order.Status = PlayerMarketOrderStatus.Confirmed;
            order.ConfirmedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<PlayerMarketOrder> CompleteOrderAsync(int orderId)
        {
            var order = await _context.PlayerMarketOrders
                .Include(o => o.Listing)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new ArgumentException("Order not found");

            if (order.Status != PlayerMarketOrderStatus.Confirmed)
                throw new InvalidOperationException("Order cannot be completed in current status");

            order.Status = PlayerMarketOrderStatus.Completed;
            order.CompletedAt = DateTime.UtcNow;

            // Update listing if it was sold out
            if (order.Listing.Status == ListingStatus.Sold)
            {
                order.Listing.Status = ListingStatus.Sold;
                order.Listing.SoldAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            var order = await _context.PlayerMarketOrders
                .Include(o => o.Listing)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return false;

            if (order.Status != PlayerMarketOrderStatus.Pending)
                return false;

            order.Status = PlayerMarketOrderStatus.Cancelled;
            order.CancelledAt = DateTime.UtcNow;

            // Restore listing available quantity
            order.Listing.AvailableQuantity += order.Quantity;
            if (order.Listing.Status == ListingStatus.Sold)
            {
                order.Listing.Status = ListingStatus.Active;
                order.Listing.SoldAt = null;
            }
            order.Listing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PlayerMarketOrder>> GetUserOrdersAsync(int userId)
        {
            return await _context.PlayerMarketOrders
                .Include(o => o.Listing)
                .Include(o => o.Seller)
                .Where(o => o.BuyerId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PlayerMarketOrder>> GetUserSalesAsync(int userId)
        {
            return await _context.PlayerMarketOrders
                .Include(o => o.Listing)
                .Include(o => o.Buyer)
                .Where(o => o.SellerId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<decimal> CalculatePlatformFeeAsync(decimal amount)
        {
            // Platform fee: 5% of transaction amount, minimum $1
            var fee = amount * 0.05m;
            return Math.Max(fee, 1.00m);
        }
    }
}