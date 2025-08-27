using GameCore.Shared.DTOs;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 玩家交易市場服務介面，定義C2C交易相關的業務邏輯
/// </summary>
public interface IPlayerMarketService
{
    /// <summary>
    /// 取得市場商品列表 (分頁查詢)
    /// </summary>
    /// <param name="query">搜尋條件</param>
    /// <returns>分頁商品列表</returns>
    Task<PagedPlayerMarketProductsDto> GetMarketProductsAsync(PlayerMarketProductSearchDto query);

    /// <summary>
    /// 取得商品詳細資訊
    /// </summary>
    /// <param name="productId">商品編號</param>
    /// <returns>商品詳細資訊，不存在則返回 null</returns>
    Task<PlayerMarketProductDto?> GetProductDetailAsync(int productId);

    /// <summary>
    /// 刊登商品到市場
    /// </summary>
    /// <param name="sellerId">賣家會員編號</param>
    /// <param name="request">刊登請求</param>
    /// <returns>刊登的商品資料</returns>
    Task<PlayerMarketProductDto> ListProductAsync(int sellerId, CreatePlayerMarketProductDto request);

    /// <summary>
    /// 更新商品資訊
    /// </summary>
    /// <param name="sellerId">賣家會員編號</param>
    /// <param name="productId">商品編號</param>
    /// <param name="request">更新請求</param>
    /// <returns>更新後的商品資料，不存在或無權限則返回 null</returns>
    Task<PlayerMarketProductDto?> UpdateProductAsync(int sellerId, int productId, CreatePlayerMarketProductDto request);

    /// <summary>
    /// 下架商品
    /// </summary>
    /// <param name="sellerId">賣家會員編號</param>
    /// <param name="productId">商品編號</param>
    /// <returns>是否下架成功</returns>
    Task<bool> RemoveProductAsync(int sellerId, int productId);

    /// <summary>
    /// 取得賣家的商品列表
    /// </summary>
    /// <param name="sellerId">賣家會員編號</param>
    /// <param name="query">查詢條件</param>
    /// <returns>分頁商品列表</returns>
    Task<PagedPlayerMarketProductsDto> GetSellerProductsAsync(int sellerId, PlayerMarketProductSearchDto query);

    /// <summary>
    /// 建立交易訂單
    /// </summary>
    /// <param name="buyerId">買家會員編號</param>
    /// <param name="request">建立訂單請求</param>
    /// <returns>建立的訂單資料</returns>
    Task<PlayerMarketOrderDto> CreateTradeOrderAsync(int buyerId, CreatePlayerMarketOrderDto request);

    /// <summary>
    /// 取得交易訂單詳細資訊
    /// </summary>
    /// <param name="userId">會員編號 (用於驗證權限)</param>
    /// <param name="orderId">訂單編號</param>
    /// <returns>訂單詳細資訊，不存在或無權限則返回 null</returns>
    Task<PlayerMarketOrderDto?> GetTradeOrderDetailAsync(int userId, int orderId);

    /// <summary>
    /// 取得用戶交易訂單列表
    /// </summary>
    /// <param name="userId">會員編號</param>
    /// <param name="query">查詢條件</param>
    /// <returns>分頁訂單列表</returns>
    Task<PagedPlayerMarketProductsDto> GetUserTradeOrdersAsync(int userId, PlayerMarketProductSearchDto query);

    /// <summary>
    /// 確認交易 (買家確認收貨)
    /// </summary>
    /// <param name="buyerId">買家會員編號</param>
    /// <param name="orderId">訂單編號</param>
    /// <returns>確認結果</returns>
    Task<ConfirmTradeActionDto> ConfirmTradeAsync(int buyerId, int orderId);

    /// <summary>
    /// 取消交易訂單
    /// </summary>
    /// <param name="userId">會員編號</param>
    /// <param name="orderId">訂單編號</param>
    /// <param name="reason">取消原因</param>
    /// <returns>是否取消成功</returns>
    Task<bool> CancelTradeOrderAsync(int userId, int orderId, string reason);

    /// <summary>
    /// 取得交易頁面資訊
    /// </summary>
    /// <param name="userId">會員編號</param>
    /// <param name="orderId">訂單編號</param>
    /// <returns>交易頁面資訊，不存在或無權限則返回 null</returns>
    Task<PlayerMarketTradepageDto?> GetTradePageAsync(int userId, int orderId);

    /// <summary>
    /// 發送交易訊息
    /// </summary>
    /// <param name="senderId">發送者會員編號</param>
    /// <param name="orderId">訂單編號</param>
    /// <param name="request">訊息請求</param>
    /// <returns>發送的訊息資料</returns>
    Task<PlayerMarketTradeMsgDto> SendTradeMessageAsync(int senderId, int orderId, SendTradeMsgDto request);

    /// <summary>
    /// 取得市場排行榜
    /// </summary>
    /// <param name="query">查詢條件</param>
    /// <returns>排行榜資料</returns>
    Task<List<PlayerMarketRankingDto>> GetMarketRankingsAsync(PlayerMarketRankingQueryDto query);

    /// <summary>
    /// 搜尋市場商品
    /// </summary>
    /// <param name="query">搜尋條件</param>
    /// <returns>分頁商品列表</returns>
    Task<PagedPlayerMarketProductsDto> SearchProductsAsync(PlayerMarketProductSearchDto query);

    /// <summary>
    /// 取得平台費用資訊
    /// </summary>
    /// <param name="amount">交易金額</param>
    /// <returns>費用資訊</returns>
    Task<decimal> GetPlatformFeeInfoAsync(decimal? amount);

    /// <summary>
    /// 取得用戶市場統計
    /// </summary>
    /// <param name="userId">會員編號</param>
    /// <returns>用戶市場統計</returns>
    Task<PlayerMarketProductDto> GetUserMarketStatsAsync(int userId);

    /// <summary>
    /// 建立舉報
    /// </summary>
    /// <param name="reporterId">舉報者會員編號</param>
    /// <param name="targetId">目標編號 (商品或訂單)</param>
    /// <param name="reportType">舉報類型</param>
    /// <param name="reason">舉報原因</param>
    /// <returns>是否建立成功</returns>
    Task<bool> CreateReportAsync(int reporterId, int targetId, string reportType, string reason);

    /// <summary>
    /// 取得推薦商品
    /// </summary>
    /// <param name="userId">會員編號</param>
    /// <param name="limit">取得數量</param>
    /// <returns>推薦商品列表</returns>
    Task<List<PlayerMarketProductDto>> GetRecommendedProductsAsync(int userId, int limit);
}