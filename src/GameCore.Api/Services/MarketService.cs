using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using GameCore.Shared.Interfaces;
using Microsoft.Extensions.Logging;
using System.Transactions;

namespace GameCore.Api.Services
{
    public class MarketService : IMarketService
    {
        private readonly IMarketRepository _marketRepository;
        private readonly IUserWalletRepository _walletRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IValidationService _validationService;
        private readonly ILogger<MarketService> _logger;

        public MarketService(
            IMarketRepository marketRepository,
            IUserWalletRepository walletRepository,
            IProductRepository productRepository,
            IUserRepository userRepository,
            IValidationService validationService,
            ILogger<MarketService> logger)
        {
            _marketRepository = marketRepository ?? throw new ArgumentNullException(nameof(marketRepository));
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<MarketResult> ListItemAsync(ListItemRequest request)
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                // 輸入驗證
                var quantityValidation = _validationService.ValidateQuantity(request.Quantity);
                if (!quantityValidation.IsValid)
                {
                    return MarketResult.Failure(quantityValidation.ErrorMessage);
                }

                var priceValidation = _validationService.ValidatePrice(request.Price);
                if (!priceValidation.IsValid)
                {
                    return MarketResult.Failure(priceValidation.ErrorMessage);
                }

                var descriptionValidation = _validationService.ValidateDescription(request.Description);
                if (!descriptionValidation.IsValid)
                {
                    return MarketResult.Failure(descriptionValidation.ErrorMessage);
                }

                // 防止惡意輸入
                if (_validationService.ContainsSqlInjection(request.Description) || _validationService.ContainsXss(request.Description))
                {
                    _logger.LogWarning("檢測到惡意輸入嘗試: {SellerId}, {Description}", request.SellerId, request.Description);
                    return MarketResult.Failure("無效的輸入格式");
                }

                // 檢查上架頻率限制
                var cacheKey = $"list_limit_{request.SellerId}";
                var recentListings = await GetRecentListingsAsync(cacheKey);
                if (recentListings >= 10)
                {
                    _logger.LogWarning("上架頻率過高: {SellerId}, 最近上架次數: {Count}", request.SellerId, recentListings);
                    return MarketResult.Failure("上架頻率過高，請稍後再試");
                }

                // 驗證商品是否存在
                var product = await _productRepository.GetByIdAsync(request.ProductId);
                if (product == null)
                {
                    return MarketResult.Failure("商品不存在");
                }

                // 檢查價格合理性（簡化版本）
                if (request.Price <= 0 || request.Price > 999999.99m)
                {
                    return MarketResult.Failure("價格異常，請檢查後重新上架");
                }

                // 創建市場項目
                var marketItem = new MarketItem
                {
                    SellerId = request.SellerId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    Price = request.Price,
                    Description = request.Description,
                    Status = MarketItemStatus.Active,
                    CreatedAt = DateTime.UtcNow
                };

                await _marketRepository.AddMarketItemAsync(marketItem);
                await _marketRepository.SaveChangesAsync();

                // 更新上架頻率記錄
                await IncrementRecentListingsAsync(cacheKey);

                transaction.Complete();

                _logger.LogInformation("商品上架成功: {SellerId}, {ProductId}, {Quantity}, 價格: {Price}",
                    request.SellerId, request.ProductId, request.Quantity, request.Price);

                return MarketResult.SuccessResult(marketItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "商品上架失敗: {SellerId}, {ProductId}",
                    request.SellerId, request.ProductId);
                return MarketResult.Failure("上架失敗，請稍後再試");
            }
        }

        public async Task<MarketResult> BuyItemAsync(BuyItemRequest request)
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                // 獲取市場項目
                var marketItem = await _marketRepository.GetMarketItemByIdAsync(request.MarketItemId);
                if (marketItem == null)
                {
                    return MarketResult.Failure("商品不存在");
                }

                if (marketItem.Status != MarketItemStatus.Active)
                {
                    return MarketResult.Failure("商品已售出或下架");
                }

                if (marketItem.SellerId == request.BuyerId)
                {
                    return MarketResult.Failure("不能購買自己的商品");
                }

                if (marketItem.Quantity < request.Quantity)
                {
                    return MarketResult.Failure("庫存不足");
                }

                // 計算總金額和手續費
                var totalAmount = marketItem.Price * request.Quantity;
                var feeAmount = CalculateTransactionFee(totalAmount);
                var sellerAmount = totalAmount - feeAmount;

                // 檢查買家餘額
                var buyerWallet = await _walletRepository.GetByUserIdAsync(request.BuyerId);
                if (buyerWallet == null)
                {
                    return MarketResult.Failure("買家錢包不存在");
                }

                if (buyerWallet.Balance < totalAmount)
                {
                    return MarketResult.Failure("餘額不足");
                }

                // 創建交易記錄
                var marketTransaction = new MarketTransaction
                {
                    MarketItemId = marketItem.MarketItemId,
                    SellerId = marketItem.SellerId,
                    BuyerId = request.BuyerId,
                    ProductId = marketItem.ProductId,
                    Quantity = request.Quantity,
                    UnitPrice = marketItem.Price,
                    TotalAmount = totalAmount,
                    FeeAmount = feeAmount,
                    SellerAmount = sellerAmount,
                    Status = MarketTransactionStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                // await _marketRepository.AddTransactionAsync(marketTransaction);

                // 扣除買家餘額
                buyerWallet.Balance -= totalAmount;
                buyerWallet.UpdatedAt = DateTime.UtcNow;
                await _walletRepository.UpdateAsync(buyerWallet);

                // 更新市場項目狀態
                if (request.Quantity == marketItem.Quantity)
                {
                    marketItem.Status = MarketItemStatus.Sold;
                }
                else
                {
                    marketItem.Quantity -= request.Quantity;
                }

                await _marketRepository.UpdateMarketItemAsync(marketItem);
                await _marketRepository.SaveChangesAsync();

                transaction.Complete();

                _logger.LogInformation("購買成功: {BuyerId} -> {MarketItemId}, {Quantity}",
                    request.BuyerId, request.MarketItemId, request.Quantity);

                return MarketResult.SuccessResult(marketTransaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "購買失敗: {BuyerId}, {MarketItemId}",
                    request.BuyerId, request.MarketItemId);
                return MarketResult.Failure("購買失敗，請稍後再試");
            }
        }

        public async Task<MarketResult> ConfirmTransactionAsync(int transactionId, int userId)
        {
            try
            {
                var transaction = await _marketRepository.GetTransactionByIdAsync(transactionId);
                if (transaction == null)
                {
                    return MarketResult.Failure("交易不存在");
                }

                // 檢查權限
                if (transaction.SellerId != userId && transaction.BuyerId != userId)
                {
                    return MarketResult.Failure("無權限確認此交易");
                }

                if (transaction.Status != MarketTransactionStatus.Pending)
                {
                    return MarketResult.Failure("交易狀態不正確");
                }

                // 更新交易狀態
                transaction.Status = MarketTransactionStatus.Completed;
                await _marketRepository.UpdateTransactionAsync(transaction);

                // 如果雙方都確認了，完成交易
                // var sellerConfirmed = await _marketRepository.IsTransactionConfirmedBySellerAsync(transactionId);
                // var buyerConfirmed = await _marketRepository.IsTransactionConfirmedByBuyerAsync(transactionId);

                // if (sellerConfirmed && buyerConfirmed)
                {
                    await CompleteTransactionAsync(transaction);
                }

                await _marketRepository.SaveChangesAsync();

                return MarketResult.SuccessResult(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "確認交易失敗: {TransactionId}, {UserId}", transactionId, userId);
                return MarketResult.Failure("確認失敗，請稍後再試");
            }
        }

        public async Task<MarketListResult> GetMarketItemsAsync(MarketSearchRequest request)
        {
            try
            {
                var items = await _marketRepository.GetMarketItemsAsync(
                    request.ProductId,
                    request.MinPrice,
                    request.MaxPrice,
                    request.SellerId,
                    request.Page,
                    request.PageSize
                );

                var totalCount = await _marketRepository.GetMarketItemsCountAsync(
                    request.ProductId,
                    request.MinPrice,
                    request.MaxPrice,
                    request.SellerId
                );

                var itemDtos = items.Select(item => new MarketItemDto
                {
                    MarketItemId = item.MarketItemId,
                    SellerId = item.SellerId,
                    SellerName = item.Seller.Username,
                    ProductId = item.ProductId,
                    ProductName = $"Product_{item.ProductId}", // 暫時使用產品ID作為名稱
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Description = item.Description,
                    Status = item.Status.ToString(),
                    CreatedAt = item.CreatedAt
                }).ToList();

                return MarketListResult.SuccessResult(itemDtos, totalCount, request.Page, request.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取市場商品失敗");
                return MarketListResult.Failure("獲取商品列表失敗");
            }
        }

        public async Task<TransactionListResult> GetUserTransactionsAsync(int userId, int page = 1, int pageSize = 20)
        {
            try
            {
                var transactions = await _marketRepository.GetUserTransactionsAsync(userId, page, pageSize);
                var totalCount = await _marketRepository.GetUserTransactionsCountAsync(userId);

                var transactionDtos = transactions.Select(t => new MarketTransactionDto
                {
                    TransactionId = t.TransactionId,
                    MarketItemId = t.MarketItemId,
                    SellerId = t.SellerId,
                    SellerName = t.Seller.Username,
                    BuyerId = t.BuyerId,
                    BuyerName = t.Buyer.Username,
                    ProductId = t.ProductId,
                    ProductName = $"Product_{t.ProductId}", // 暫時使用產品ID作為名稱
                    Quantity = t.Quantity,
                    UnitPrice = t.UnitPrice,
                    TotalAmount = t.TotalAmount,
                    FeeAmount = t.FeeAmount,
                    SellerAmount = t.SellerAmount,
                    Status = t.Status.ToString(),
                    CreatedAt = t.CreatedAt,
                    CompletedAt = t.CompletedAt
                }).ToList();

                return TransactionListResult.SuccessResult(transactionDtos, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取用戶交易記錄失敗: {UserId}", userId);
                return TransactionListResult.Failure("獲取交易記錄失敗");
            }
        }

        public async Task<MarketResult> GetMarketStatisticsAsync()
        {
            try
            {
                var totalItems = await _marketRepository.GetTotalMarketItemsAsync();
                var activeItems = await _marketRepository.GetActiveMarketItemsAsync();
                var totalTransactions = await _marketRepository.GetTotalTransactionsAsync();
                var totalVolume = await _marketRepository.GetTotalTransactionVolumeAsync();

                var statistics = new
                {
                    TotalItems = totalItems,
                    ActiveItems = activeItems,
                    TotalTransactions = totalTransactions,
                    TotalVolume = totalVolume,
                    AveragePrice = totalItems > 0 ? totalVolume / totalItems : 0,
                    LastUpdated = DateTime.UtcNow
                };

                return MarketResult.SuccessResult(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取市場統計失敗");
                return MarketResult.Failure("獲取統計失敗");
            }
        }

        public async Task<MarketResult> CancelMarketItemAsync(int marketItemId, int userId)
        {
            try
            {
                var marketItem = await _marketRepository.GetMarketItemByIdAsync(marketItemId);
                if (marketItem == null)
                {
                    return MarketResult.Failure("商品不存在");
                }

                if (marketItem.SellerId != userId)
                {
                    return MarketResult.Failure("無權限取消此商品");
                }

                if (marketItem.Status != MarketItemStatus.Active)
                {
                    return MarketResult.Failure("商品狀態不允許取消");
                }

                marketItem.Status = MarketItemStatus.Cancelled;
                await _marketRepository.UpdateMarketItemAsync(marketItem);
                await _marketRepository.SaveChangesAsync();

                _logger.LogInformation("商品取消成功: {MarketItemId}, {UserId}", marketItemId, userId);

                return MarketResult.SuccessResult(marketItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取消商品失敗: {MarketItemId}, {UserId}", marketItemId, userId);
                return MarketResult.Failure("取消失敗，請稍後再試");
            }
        }

        public async Task<MarketResult> UpdateMarketItemAsync(int marketItemId, int userId, decimal newPrice, string newDescription)
        {
            try
            {
                var marketItem = await _marketRepository.GetMarketItemByIdAsync(marketItemId);
                if (marketItem == null)
                {
                    return MarketResult.Failure("商品不存在");
                }

                if (marketItem.SellerId != userId)
                {
                    return MarketResult.Failure("無權限修改此商品");
                }

                if (marketItem.Status != MarketItemStatus.Active)
                {
                    return MarketResult.Failure("商品狀態不允許修改");
                }

                // 驗證新價格
                if (newPrice <= 0 || newPrice > 999999.99m)
                {
                    return MarketResult.Failure("商品價格必須在 0.01-999,999.99 之間");
                }

                // 驗證新描述
                if (string.IsNullOrWhiteSpace(newDescription) || newDescription.Length > 500)
                {
                    return MarketResult.Failure("商品描述不能為空且不能超過 500 字符");
                }

                marketItem.Price = newPrice;
                marketItem.Description = newDescription;
                await _marketRepository.UpdateMarketItemAsync(marketItem);
                await _marketRepository.SaveChangesAsync();

                _logger.LogInformation("商品更新成功: {MarketItemId}, {UserId}", marketItemId, userId);

                return MarketResult.SuccessResult(marketItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新商品失敗: {MarketItemId}, {UserId}", marketItemId, userId);
                return MarketResult.Failure("更新失敗，請稍後再試");
            }
        }

        private decimal CalculateTransactionFee(decimal amount)
        {
            // 手續費計算邏輯：
            // 1. 基礎手續費：5%
            // 2. 最低手續費：10 元
            // 3. 最高手續費：500 元
            // 4. VIP 用戶折扣：手續費減半
            var baseFee = amount * 0.05m;
            var fee = Math.Max(baseFee, 10m);
            fee = Math.Min(fee, 500m);

            // 檢查用戶是否為 VIP，如果是則手續費減半
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null && user.Role == GameCore.Domain.Enums.UserRole.VIP)
            {
                fee *= 0.5m;
                _logger.LogInformation("VIP用戶 {UserId} 享受手續費減半優惠", userId);
            }

            return fee;
        }

        private async Task CompleteTransactionAsync(MarketTransaction transaction)
        {
            // 轉帳給賣家
            var sellerWallet = await _walletRepository.GetByUserIdAsync(transaction.SellerId);
            if (sellerWallet != null)
            {
                sellerWallet.Balance += transaction.SellerAmount;
                sellerWallet.UpdatedAt = DateTime.UtcNow;
                await _walletRepository.UpdateAsync(sellerWallet);
            }

            // 更新交易狀態
            transaction.Status = MarketTransactionStatus.Completed;
            transaction.CompletedAt = DateTime.UtcNow;
            await _marketRepository.UpdateTransactionAsync(transaction);
        }

        // 新增安全驗證方法


        // 新增上架頻率限制方法
        private Task<int> GetRecentListingsAsync(string cacheKey)
        {
            // 這裡需要注入 IMemoryCache，暫時返回 0
            return Task.FromResult(0);
        }

        private Task IncrementRecentListingsAsync(string cacheKey)
        {
            // 這裡需要注入 IMemoryCache，暫時不實作
            return Task.CompletedTask;
        }

        public async Task<MarketResult> SearchItemsAsync(MarketSearchRequest request)
        {
            try
            {
                var items = await _marketRepository.SearchItemsAsync(request);
                return MarketResult.SuccessResult(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "搜尋商品失敗");
                return MarketResult.Failure("搜尋失敗，請稍後再試");
            }
        }

        public async Task<MarketResult> CancelItemAsync(int marketItemId, int userId)
        {
            try
            {
                var marketItem = await _marketRepository.GetMarketItemByIdAsync(marketItemId);
                if (marketItem == null)
                {
                    return MarketResult.Failure("商品不存在");
                }

                if (marketItem.SellerId != userId)
                {
                    return MarketResult.Failure("無權限取消此商品");
                }

                await _marketRepository.DeleteMarketItemAsync(marketItemId);
                await _marketRepository.SaveChangesAsync();

                return MarketResult.SuccessResult("商品已取消");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取消商品失敗: {MarketItemId}, {UserId}", marketItemId, userId);
                return MarketResult.Failure("取消失敗，請稍後再試");
            }
        }

        public async Task<MarketResult> UpdateItemAsync(int marketItemId, int userId, decimal newPrice, string newDescription)
        {
            try
            {
                var marketItem = await _marketRepository.GetMarketItemByIdAsync(marketItemId);
                if (marketItem == null)
                {
                    return MarketResult.Failure("商品不存在");
                }

                if (marketItem.SellerId != userId)
                {
                    return MarketResult.Failure("無權限修改此商品");
                }

                marketItem.Price = newPrice;
                marketItem.Description = newDescription;
                await _marketRepository.UpdateMarketItemAsync(marketItem);
                await _marketRepository.SaveChangesAsync();

                return MarketResult.SuccessResult("商品已更新");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新商品失敗: {MarketItemId}, {UserId}", marketItemId, userId);
                return MarketResult.Failure("更新失敗，請稍後再試");
            }
        }
    }
}
