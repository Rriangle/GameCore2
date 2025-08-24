using GameCore.Api.DTOs;
using GameCore.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace GameCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MarketController : ControllerBase
{
    private readonly IMarketService _marketService;
    private readonly ILogger<MarketController> _logger;

    public MarketController(IMarketService marketService, ILogger<MarketController> logger)
    {
        _marketService = marketService;
        _logger = logger;
    }

    /// <summary>
    /// 獲取市場商品列表
    /// </summary>
    /// <param name="category">商品分類</param>
    /// <returns>商品列表</returns>
    [HttpGet("items")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MarketItemDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetMarketItems([FromQuery] string? category)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取市場商品列表請求: {CorrelationId}, Category: {Category}", correlationId, category);

        var items = await _marketService.GetMarketItemsAsync(category);

        _logger.LogInformation("成功獲取市場商品列表: {CorrelationId}, Count: {Count}", correlationId, items.Count());
        return Ok(ApiResponse<IEnumerable<MarketItemDto>>.SuccessResult(items));
    }

    /// <summary>
    /// 獲取市場商品詳情
    /// </summary>
    /// <param name="itemId">商品ID</param>
    /// <returns>商品詳情</returns>
    [HttpGet("items/{itemId}")]
    [ProducesResponseType(typeof(ApiResponse<MarketItemDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetMarketItem(int itemId)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取市場商品詳情請求: {CorrelationId}, ItemId: {ItemId}", correlationId, itemId);

        var item = await _marketService.GetMarketItemAsync(itemId);
        if (item == null)
        {
            _logger.LogWarning("市場商品不存在: {CorrelationId}, ItemId: {ItemId}", correlationId, itemId);
            return NotFound(ApiResponse<object>.ErrorResult("商品不存在"));
        }

        _logger.LogInformation("成功獲取市場商品詳情: {CorrelationId}, ItemId: {ItemId}", correlationId, itemId);
        return Ok(ApiResponse<MarketItemDto>.SuccessResult(item));
    }

    /// <summary>
    /// 創建市場商品
    /// </summary>
    /// <param name="request">商品創建請求</param>
    /// <returns>創建結果</returns>
    [HttpPost("items")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<MarketItemResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> CreateMarketItem([FromBody] CreateMarketItemRequestDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到創建市場商品請求: {CorrelationId}, Name: {Name}, Price: {Price}", 
            correlationId, request.Name, request.Price);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("創建市場商品請求驗證失敗: {CorrelationId}, Errors: {Errors}", correlationId, string.Join(", ", errors));
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _marketService.CreateMarketItemAsync(userId, request.Name, request.Description, 
            request.Price, request.ImageUrl, request.Category);

        if (result.Success)
        {
            _logger.LogInformation("市場商品創建成功: {CorrelationId}, UserId: {UserId}, ItemId: {ItemId}", 
                correlationId, userId, result.Item?.ItemId);
            return Ok(ApiResponse<MarketItemResult>.SuccessResult(result));
        }

        _logger.LogWarning("市場商品創建失敗: {CorrelationId}, UserId: {UserId}, Message: {Message}", 
            correlationId, userId, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "商品創建失敗"));
    }

    /// <summary>
    /// 更新市場商品
    /// </summary>
    /// <param name="itemId">商品ID</param>
    /// <param name="request">更新請求</param>
    /// <returns>更新結果</returns>
    [HttpPut("items/{itemId}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<MarketItemResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> UpdateMarketItem(int itemId, [FromBody] UpdateMarketItemRequestDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到更新市場商品請求: {CorrelationId}, ItemId: {ItemId}", correlationId, itemId);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("更新市場商品請求驗證失敗: {CorrelationId}, Errors: {Errors}", correlationId, string.Join(", ", errors));
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _marketService.UpdateMarketItemAsync(itemId, userId, request.Name, request.Description, 
            request.Price, request.ImageUrl, request.Category);

        if (result.Success)
        {
            _logger.LogInformation("市場商品更新成功: {CorrelationId}, ItemId: {ItemId}, UserId: {UserId}", 
                correlationId, itemId, userId);
            return Ok(ApiResponse<MarketItemResult>.SuccessResult(result));
        }

        _logger.LogWarning("市場商品更新失敗: {CorrelationId}, ItemId: {ItemId}, UserId: {UserId}, Message: {Message}", 
            correlationId, itemId, userId, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "商品更新失敗"));
    }

    /// <summary>
    /// 取消市場商品
    /// </summary>
    /// <param name="itemId">商品ID</param>
    /// <returns>取消結果</returns>
    [HttpDelete("items/{itemId}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<MarketItemResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> CancelMarketItem(int itemId)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到取消市場商品請求: {CorrelationId}, ItemId: {ItemId}", correlationId, itemId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _marketService.CancelMarketItemAsync(itemId, userId);

        if (result.Success)
        {
            _logger.LogInformation("市場商品取消成功: {CorrelationId}, ItemId: {ItemId}, UserId: {UserId}", 
                correlationId, itemId, userId);
            return Ok(ApiResponse<MarketItemResult>.SuccessResult(result));
        }

        _logger.LogWarning("市場商品取消失敗: {CorrelationId}, ItemId: {ItemId}, UserId: {UserId}, Message: {Message}", 
            correlationId, itemId, userId, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "商品取消失敗"));
    }

    /// <summary>
    /// 購買商品
    /// </summary>
    /// <param name="request">購買請求</param>
    /// <returns>購買結果</returns>
    [HttpPost("purchase")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<MarketTransactionResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> PurchaseItem([FromBody] PurchaseItemRequestDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到購買商品請求: {CorrelationId}, ItemId: {ItemId}", correlationId, request.ItemId);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("購買商品請求驗證失敗: {CorrelationId}, Errors: {Errors}", correlationId, string.Join(", ", errors));
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _marketService.PurchaseItemAsync(request.ItemId, userId);

        if (result.Success)
        {
            _logger.LogInformation("商品購買成功: {CorrelationId}, UserId: {UserId}, TransactionId: {TransactionId}", 
                correlationId, userId, result.Transaction?.TransactionId);
            return Ok(ApiResponse<MarketTransactionResult>.SuccessResult(result));
        }

        _logger.LogWarning("商品購買失敗: {CorrelationId}, UserId: {UserId}, Message: {Message}", 
            correlationId, userId, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "購買失敗"));
    }

    /// <summary>
    /// 確認交易
    /// </summary>
    /// <param name="request">確認請求</param>
    /// <returns>確認結果</returns>
    [HttpPost("confirm")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<MarketTransactionResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> ConfirmTransaction([FromBody] ConfirmTransactionRequestDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到確認交易請求: {CorrelationId}, TransactionId: {TransactionId}, IsSeller: {IsSeller}", 
            correlationId, request.TransactionId, request.IsSeller);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("確認交易請求驗證失敗: {CorrelationId}, Errors: {Errors}", correlationId, string.Join(", ", errors));
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _marketService.ConfirmTransactionAsync(request.TransactionId, userId, request.IsSeller);

        if (result.Success)
        {
            _logger.LogInformation("交易確認成功: {CorrelationId}, TransactionId: {TransactionId}, UserId: {UserId}", 
                correlationId, request.TransactionId, userId);
            return Ok(ApiResponse<MarketTransactionResult>.SuccessResult(result));
        }

        _logger.LogWarning("交易確認失敗: {CorrelationId}, TransactionId: {TransactionId}, UserId: {UserId}, Message: {Message}", 
            correlationId, request.TransactionId, userId, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "交易確認失敗"));
    }

    /// <summary>
    /// 獲取用戶交易記錄
    /// </summary>
    /// <returns>交易記錄列表</returns>
    [HttpGet("transactions")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MarketTransactionDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetUserTransactions()
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取用戶交易記錄請求: {CorrelationId}", correlationId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var transactions = await _marketService.GetUserTransactionsAsync(userId);

        _logger.LogInformation("成功獲取用戶交易記錄: {CorrelationId}, UserId: {UserId}, Count: {Count}", 
            correlationId, userId, transactions.Count());
        return Ok(ApiResponse<IEnumerable<MarketTransactionDto>>.SuccessResult(transactions));
    }
}

public class CreateMarketItemRequestDto
{
    [Required(ErrorMessage = "商品名稱為必填項目")]
    [StringLength(100, ErrorMessage = "商品名稱長度不能超過 100 個字元")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "商品描述為必填項目")]
    [StringLength(500, ErrorMessage = "商品描述長度不能超過 500 個字元")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "商品價格為必填項目")]
    [Range(0.01, 999999.99, ErrorMessage = "商品價格必須在 0.01-999999.99 之間")]
    public decimal Price { get; set; }

    [StringLength(200, ErrorMessage = "圖片URL長度不能超過 200 個字元")]
    public string? ImageUrl { get; set; }

    [StringLength(20, ErrorMessage = "商品分類長度不能超過 20 個字元")]
    public string? Category { get; set; }
}

public class UpdateMarketItemRequestDto
{
    [Required(ErrorMessage = "商品名稱為必填項目")]
    [StringLength(100, ErrorMessage = "商品名稱長度不能超過 100 個字元")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "商品描述為必填項目")]
    [StringLength(500, ErrorMessage = "商品描述長度不能超過 500 個字元")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "商品價格為必填項目")]
    [Range(0.01, 999999.99, ErrorMessage = "商品價格必須在 0.01-999999.99 之間")]
    public decimal Price { get; set; }

    [StringLength(200, ErrorMessage = "圖片URL長度不能超過 200 個字元")]
    public string? ImageUrl { get; set; }

    [StringLength(20, ErrorMessage = "商品分類長度不能超過 20 個字元")]
    public string? Category { get; set; }
}

public class PurchaseItemRequestDto
{
    [Required(ErrorMessage = "商品ID為必填項目")]
    public int ItemId { get; set; }
}

public class ConfirmTransactionRequestDto
{
    [Required(ErrorMessage = "交易ID為必填項目")]
    public int TransactionId { get; set; }

    [Required(ErrorMessage = "是否為賣家為必填項目")]
    public bool IsSeller { get; set; }
}