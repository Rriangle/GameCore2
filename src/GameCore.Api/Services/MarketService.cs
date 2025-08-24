using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Api.Services;

public class MarketService : IMarketService
{
    private readonly GameCoreDbContext _context;
    private readonly IUserWalletRepository _walletRepository;
    private readonly ILogger<MarketService> _logger;

    public MarketService(GameCoreDbContext context, IUserWalletRepository walletRepository, ILogger<MarketService> logger)
    {
        _context = context;
        _walletRepository = walletRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<MarketItemDto>> GetMarketItemsAsync(string? category = null)
    {
        try
        {
            var query = _context.PlayerOwneds
                .Include(p => p.User)
                .Where(p => p.Status == "Active");

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
            }

            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return items.Select(p => new MarketItemDto
            {
                ItemId = p.ItemId,
                SellerId = p.UserId,
                SellerName = p.User.Username,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Status = p.Status,
                ImageUrl = p.ImageUrl,
                Category = p.Category,
                CreatedAt = p.CreatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting market items");
            return Enumerable.Empty<MarketItemDto>();
        }
    }

    public async Task<MarketItemDto?> GetMarketItemAsync(int itemId)
    {
        try
        {
            var item = await _context.PlayerOwneds
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.ItemId == itemId);

            if (item == null) return null;

            return new MarketItemDto
            {
                ItemId = item.ItemId,
                SellerId = item.UserId,
                SellerName = item.User.Username,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                Status = item.Status,
                ImageUrl = item.ImageUrl,
                Category = item.Category,
                CreatedAt = item.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting market item {ItemId}", itemId);
            return null;
        }
    }

    public async Task<MarketItemResult> CreateMarketItemAsync(int userId, string name, string description, decimal price, string? imageUrl, string? category)
    {
        try
        {
            var item = new PlayerOwned
            {
                UserId = userId,
                Name = name,
                Description = description,
                Price = price,
                Status = "Active",
                ImageUrl = imageUrl,
                Category = category,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.PlayerOwneds.Add(item);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Market item created: User {UserId}, Name {Name}, Price {Price}", userId, name, price);

            return new MarketItemResult
            {
                Success = true,
                Message = "商品上架成功",
                Item = new MarketItemDto
                {
                    ItemId = item.ItemId,
                    SellerId = item.UserId,
                    SellerName = "當前用戶", // Will be populated when retrieved
                    Name = item.Name,
                    Description = item.Description,
                    Price = item.Price,
                    Status = item.Status,
                    ImageUrl = item.ImageUrl,
                    Category = item.Category,
                    CreatedAt = item.CreatedAt
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating market item for user {UserId}", userId);
            return new MarketItemResult
            {
                Success = false,
                Message = "商品上架失敗"
            };
        }
    }

    public async Task<MarketItemResult> UpdateMarketItemAsync(int itemId, int userId, string name, string description, decimal price, string? imageUrl, string? category)
    {
        try
        {
            var item = await _context.PlayerOwneds
                .FirstOrDefaultAsync(p => p.ItemId == itemId && p.UserId == userId && p.Status == "Active");

            if (item == null)
            {
                return new MarketItemResult
                {
                    Success = false,
                    Message = "商品不存在或無權限修改"
                };
            }

            item.Name = name;
            item.Description = description;
            item.Price = price;
            item.ImageUrl = imageUrl;
            item.Category = category;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Market item updated: Item {ItemId}, User {UserId}", itemId, userId);

            return new MarketItemResult
            {
                Success = true,
                Message = "商品更新成功",
                Item = await GetMarketItemAsync(itemId)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating market item {ItemId} for user {UserId}", itemId, userId);
            return new MarketItemResult
            {
                Success = false,
                Message = "商品更新失敗"
            };
        }
    }

    public async Task<MarketItemResult> CancelMarketItemAsync(int itemId, int userId)
    {
        try
        {
            var item = await _context.PlayerOwneds
                .FirstOrDefaultAsync(p => p.ItemId == itemId && p.UserId == userId && p.Status == "Active");

            if (item == null)
            {
                return new MarketItemResult
                {
                    Success = false,
                    Message = "商品不存在或無權限取消"
                };
            }

            item.Status = "Cancelled";
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Market item cancelled: Item {ItemId}, User {UserId}", itemId, userId);

            return new MarketItemResult
            {
                Success = true,
                Message = "商品下架成功",
                Item = await GetMarketItemAsync(itemId)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling market item {ItemId} for user {UserId}", itemId, userId);
            return new MarketItemResult
            {
                Success = false,
                Message = "商品下架失敗"
            };
        }
    }

    public async Task<MarketTransactionResult> PurchaseItemAsync(int itemId, int buyerId)
    {
        try
        {
            var item = await _context.PlayerOwneds
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.ItemId == itemId && p.Status == "Active");

            if (item == null)
            {
                return new MarketTransactionResult
                {
                    Success = false,
                    Message = "商品不存在或已售出"
                };
            }

            if (item.UserId == buyerId)
            {
                return new MarketTransactionResult
                {
                    Success = false,
                    Message = "不能購買自己的商品"
                };
            }

            // Check if buyer has enough points
            var buyerWallet = await _walletRepository.GetByUserIdAsync(buyerId);
            if (buyerWallet == null || buyerWallet.Balance < item.Price)
            {
                return new MarketTransactionResult
                {
                    Success = false,
                    Message = "點數不足"
                };
            }

            // Calculate platform fee (5%)
            var platformFee = item.Price * 0.05m;
            var sellerAmount = item.Price - platformFee;

            // Create transaction
            var transaction = new PlayerOwnedToPlayer
            {
                ItemId = itemId,
                SellerId = item.UserId,
                BuyerId = buyerId,
                Price = item.Price,
                PlatformFee = platformFee,
                OrderStatus = "Created",
                PaymentStatus = "Pending",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.PlayerOwnedToPlayers.Add(transaction);

            // Update item status
            item.Status = "Sold";
            item.SoldAt = DateTime.UtcNow;
            item.UpdatedAt = DateTime.UtcNow;

            // Deduct points from buyer
            buyerWallet.Balance -= item.Price;
            buyerWallet.UpdatedAt = DateTime.UtcNow;
            await _walletRepository.UpdateAsync(buyerWallet);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Market purchase: Item {ItemId}, Buyer {BuyerId}, Seller {SellerId}, Price {Price}", 
                itemId, buyerId, item.UserId, item.Price);

            return new MarketTransactionResult
            {
                Success = true,
                Message = "購買成功，等待賣家確認",
                Transaction = await MapToTransactionDto(transaction)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error purchasing item {ItemId} by user {BuyerId}", itemId, buyerId);
            return new MarketTransactionResult
            {
                Success = false,
                Message = "購買失敗"
            };
        }
    }

    public async Task<MarketTransactionResult> ConfirmTransactionAsync(int transactionId, int userId, bool isSeller)
    {
        try
        {
            var transaction = await _context.PlayerOwnedToPlayers
                .Include(t => t.Item)
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);

            if (transaction == null)
            {
                return new MarketTransactionResult
                {
                    Success = false,
                    Message = "交易不存在"
                };
            }

            // Verify user is part of the transaction
            if ((isSeller && transaction.SellerId != userId) || (!isSeller && transaction.BuyerId != userId))
            {
                return new MarketTransactionResult
                {
                    Success = false,
                    Message = "無權限確認此交易"
                };
            }

            // Update confirmation timestamp
            if (isSeller)
            {
                transaction.SellerConfirmedAt = DateTime.UtcNow;
            }
            else
            {
                transaction.BuyerConfirmedAt = DateTime.UtcNow;
            }

            // Check if both parties have confirmed
            if (transaction.SellerConfirmedAt.HasValue && transaction.BuyerConfirmedAt.HasValue)
            {
                transaction.OrderStatus = "Completed";
                transaction.CompletedAt = DateTime.UtcNow;

                // Credit seller's wallet
                var sellerWallet = await _walletRepository.GetByUserIdAsync(transaction.SellerId);
                if (sellerWallet != null)
                {
                    var sellerAmount = transaction.Price - transaction.PlatformFee;
                    sellerWallet.Balance += sellerAmount;
                    sellerWallet.UpdatedAt = DateTime.UtcNow;
                    await _walletRepository.UpdateAsync(sellerWallet);
                }
            }
            else
            {
                transaction.OrderStatus = "Trading";
            }

            transaction.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Transaction confirmed: {TransactionId}, User {UserId}, IsSeller {IsSeller}", 
                transactionId, userId, isSeller);

            return new MarketTransactionResult
            {
                Success = true,
                Message = "交易確認成功",
                Transaction = await MapToTransactionDto(transaction)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming transaction {TransactionId} by user {UserId}", transactionId, userId);
            return new MarketTransactionResult
            {
                Success = false,
                Message = "交易確認失敗"
            };
        }
    }

    public async Task<IEnumerable<MarketTransactionDto>> GetUserTransactionsAsync(int userId)
    {
        try
        {
            var transactions = await _context.PlayerOwnedToPlayers
                .Include(t => t.Item)
                .Include(t => t.Seller)
                .Include(t => t.Buyer)
                .Where(t => t.SellerId == userId || t.BuyerId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            var transactionDtos = new List<MarketTransactionDto>();
            foreach (var transaction in transactions)
            {
                transactionDtos.Add(await MapToTransactionDto(transaction));
            }

            return transactionDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transactions for user {UserId}", userId);
            return Enumerable.Empty<MarketTransactionDto>();
        }
    }

    public async Task<MarketTransactionDto?> GetTransactionAsync(int transactionId)
    {
        try
        {
            var transaction = await _context.PlayerOwnedToPlayers
                .Include(t => t.Item)
                .Include(t => t.Seller)
                .Include(t => t.Buyer)
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);

            return transaction != null ? await MapToTransactionDto(transaction) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transaction {TransactionId}", transactionId);
            return null;
        }
    }

    private async Task<MarketTransactionDto> MapToTransactionDto(PlayerOwnedToPlayer transaction)
    {
        return new MarketTransactionDto
        {
            TransactionId = transaction.TransactionId,
            ItemId = transaction.ItemId,
            ItemName = transaction.Item?.Name ?? "未知商品",
            SellerId = transaction.SellerId,
            SellerName = transaction.Seller?.Username ?? "未知賣家",
            BuyerId = transaction.BuyerId,
            BuyerName = transaction.Buyer?.Username ?? "未知買家",
            Price = transaction.Price,
            PlatformFee = transaction.PlatformFee,
            OrderStatus = transaction.OrderStatus,
            PaymentStatus = transaction.PaymentStatus,
            SellerConfirmedAt = transaction.SellerConfirmedAt,
            BuyerConfirmedAt = transaction.BuyerConfirmedAt,
            CompletedAt = transaction.CompletedAt,
            CreatedAt = transaction.CreatedAt
        };
    }
}