using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using System.Security.Claims;

namespace GameCore.Mvc.Controllers;

/// <summary>
/// 玩家交易市場控制器 - C2C交易系統
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PlayerMarketController : ControllerBase
{
    private readonly IPlayerMarketService _playerMarketService;
    private readonly ILogger<PlayerMarketController> _logger;

    public PlayerMarketController(IPlayerMarketService playerMarketService, ILogger<PlayerMarketController> logger)
    {
        _playerMarketService = playerMarketService;
        _logger = logger;
    }

    /// <summary>
    /// 獲取市場商品列表
    /// </summary>
    [HttpGet("products")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedPlayerMarketProductsDto>> GetMarketProducts([FromQuery] PlayerMarketProductSearchDto query)
    {
        try
        {
            var result = await _playerMarketService.GetMarketProductsAsync(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取市場商品列表時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取商品詳情
    /// </summary>
    [HttpGet("products/{productId}")]
    [AllowAnonymous]
    public async Task<ActionResult<PlayerMarketProductDto>> GetProductDetail(int productId)
    {
        try
        {
            var result = await _playerMarketService.GetProductDetailAsync(productId);
            if (result == null)
                return NotFound(new { message = "商品不存在" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取商品詳情時發生錯誤: {ProductId}", productId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 刊登商品到市場
    /// </summary>
    [HttpPost("products")]
    public async Task<ActionResult<PlayerMarketProductDto>> ListProduct([FromBody] CreatePlayerMarketProductDto request)
    {
        try
        {
            var sellerId = GetCurrentUserId();
            var result = await _playerMarketService.ListProductAsync(sellerId, request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刊登商品時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 更新商品信息
    /// </summary>
    [HttpPut("products/{productId}")]
    public async Task<ActionResult<PlayerMarketProductDto>> UpdateProduct(int productId, [FromBody] CreatePlayerMarketProductDto request)
    {
        try
        {
            var sellerId = GetCurrentUserId();
            var result = await _playerMarketService.UpdateProductAsync(sellerId, productId, request);
            if (result == null)
                return NotFound(new { message = "商品不存在或無權限修改" });

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新商品時發生錯誤: {ProductId}", productId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 下架商品
    /// </summary>
    [HttpDelete("products/{productId}")]
    public async Task<ActionResult> RemoveProduct(int productId)
    {
        try
        {
            var sellerId = GetCurrentUserId();
            var result = await _playerMarketService.RemoveProductAsync(sellerId, productId);
            if (!result)
                return NotFound(new { message = "商品不存在或無權限下架" });

            return Ok(new { message = "商品已成功下架" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "下架商品時發生錯誤: {ProductId}", productId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取賣家的商品列表
    /// </summary>
    [HttpGet("my-products")]
    public async Task<ActionResult<PagedPlayerMarketProductsDto>> GetMyProducts([FromQuery] PlayerMarketProductSearchDto query)
    {
        try
        {
            var sellerId = GetCurrentUserId();
            var result = await _playerMarketService.GetSellerProductsAsync(sellerId, query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取賣家商品列表時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 創建交易訂單
    /// </summary>
    [HttpPost("orders")]
    public async Task<ActionResult<PlayerMarketOrderDto>> CreateOrder([FromBody] CreatePlayerMarketOrderDto request)
    {
        try
        {
            var buyerId = GetCurrentUserId();
            var result = await _playerMarketService.CreateTradeOrderAsync(buyerId, request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "創建交易訂單時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取交易訂單詳情
    /// </summary>
    [HttpGet("orders/{orderId}")]
    public async Task<ActionResult<PlayerMarketOrderDto>> GetOrderDetail(int orderId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _playerMarketService.GetTradeOrderDetailAsync(userId, orderId);
            if (result == null)
                return NotFound(new { message = "訂單不存在或無權限查看" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取交易訂單詳情時發生錯誤: {OrderId}", orderId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取用戶交易訂單列表
    /// </summary>
    [HttpGet("orders")]
    public async Task<ActionResult<PagedPlayerMarketProductsDto>> GetOrders([FromQuery] PlayerMarketProductSearchDto query)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _playerMarketService.GetUserTradeOrdersAsync(userId, query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取交易訂單列表時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 確認交易（買家確認收貨）
    /// </summary>
    [HttpPost("orders/{orderId}/confirm")]
    public async Task<ActionResult> ConfirmTrade(int orderId)
    {
        try
        {
            var buyerId = GetCurrentUserId();
                    var result = await _playerMarketService.ConfirmTradeAsync(buyerId, orderId);
        return Ok(new { message = "交易確認成功", result = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "確認交易時發生錯誤: {OrderId}", orderId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 取消交易訂單
    /// </summary>
    [HttpPost("orders/{orderId}/cancel")]
    public async Task<ActionResult> CancelOrder(int orderId, [FromBody] CancelOrderRequestDto request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _playerMarketService.CancelTradeOrderAsync(userId, orderId, request.Reason);
            if (!result)
                return BadRequest(new { message = "無法取消此訂單" });

            return Ok(new { message = "訂單已成功取消" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取消交易訂單時發生錯誤: {OrderId}", orderId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取交易頁面信息（買賣雙方溝通頁面）
    /// </summary>
    [HttpGet("orders/{orderId}/trade-page")]
    public async Task<ActionResult<PlayerMarketTradepageDto>> GetTradePage(int orderId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _playerMarketService.GetTradePageAsync(userId, orderId);
            if (result == null)
                return NotFound(new { message = "交易頁面不存在或無權限訪問" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取交易頁面時發生錯誤: {OrderId}", orderId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 發送交易訊息
    /// </summary>
    [HttpPost("orders/{orderId}/messages")]
    public async Task<ActionResult<PlayerMarketTradeMsgDto>> SendTradeMessage(int orderId, [FromBody] SendTradeMsgDto request)
    {
        try
        {
            var senderId = GetCurrentUserId();
            var result = await _playerMarketService.SendTradeMessageAsync(senderId, orderId, request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "發送交易訊息時發生錯誤: {OrderId}", orderId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取市場排行榜
    /// </summary>
    [HttpGet("rankings")]
    [AllowAnonymous]
    public async Task<ActionResult<List<PlayerMarketRankingDto>>> GetMarketRankings([FromQuery] PlayerMarketRankingQueryDto query)
    {
        try
        {
            var result = await _playerMarketService.GetMarketRankingsAsync(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取市場排行榜時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 搜尋市場商品
    /// </summary>
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedPlayerMarketProductsDto>> SearchProducts([FromQuery] string keyword, [FromQuery] PlayerMarketProductSearchDto query)
    {
        try
        {
            query.Keyword = keyword;
            var result = await _playerMarketService.SearchProductsAsync(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "搜尋市場商品時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取平台費用信息
    /// </summary>
    [HttpGet("fee-info")]
    [AllowAnonymous]
    public async Task<ActionResult<decimal>> GetFeeInfo([FromQuery] decimal? amount = null)
    {
        try
        {
            var result = await _playerMarketService.GetPlatformFeeInfoAsync(amount);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取平台費用信息時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取用戶市場統計
    /// </summary>
    [HttpGet("my-stats")]
    public async Task<ActionResult<PlayerMarketProductDto>> GetMyStats()
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _playerMarketService.GetUserMarketStatsAsync(userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取用戶市場統計時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 舉報商品或交易
    /// </summary>
    [HttpPost("reports")]
    public async Task<ActionResult> CreateReport([FromBody] CreateReportRequestDto request)
    {
        try
        {
            var reporterId = GetCurrentUserId();
            var result = await _playerMarketService.CreateReportAsync(reporterId, request.TargetId, request.ReportType, request.Reason);
            if (result)
                return Ok(new { message = "舉報已提交，感謝您的反饋" });
            else
                return BadRequest(new { message = "舉報提交失敗" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "創建舉報時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取推薦商品
    /// </summary>
    [HttpGet("recommendations")]
    public async Task<ActionResult<List<PlayerMarketProductDto>>> GetRecommendations([FromQuery] int limit = 10)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _playerMarketService.GetRecommendedProductsAsync(userId, limit);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取推薦商品時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            throw new UnauthorizedAccessException("無效的用戶身份");
        }
        return userId;
    }
}

/// <summary>
/// 取消訂單請求 DTO
/// </summary>
public class CancelOrderRequestDto
{
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// 發送交易訊息請求 DTO
/// </summary>
public class SendTradeMessageRequestDto
{
    public string Message { get; set; } = string.Empty;
    public string? MessageType { get; set; }
    public string? AttachmentUrl { get; set; }
}

/// <summary>
/// 創建舉報請求 DTO
/// </summary>
public class CreateReportRequestDto
{
    public string ReportType { get; set; } = string.Empty; // "product" or "trade"
    public int TargetId { get; set; } // ProductId or OrderId
    public string Reason { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string>? Evidence { get; set; }
}