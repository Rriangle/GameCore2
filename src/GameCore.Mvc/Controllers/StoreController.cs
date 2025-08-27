using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using System.Security.Claims;

namespace GameCore.Mvc.Controllers;

/// <summary>
/// 官方商城控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StoreController : ControllerBase
{
    private readonly IStoreService _storeService;
    private readonly ILogger<StoreController> _logger;

    public StoreController(IStoreService storeService, ILogger<StoreController> logger)
    {
        _storeService = storeService;
        _logger = logger;
    }

    /// <summary>
    /// 獲取商品列表
    /// </summary>
    [HttpGet("products")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedProductsDto>> GetProducts([FromQuery] ProductSearchQueryDto query)
    {
        try
        {
            var result = await _storeService.SearchProductsAsync(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取商品列表時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取商品詳情
    /// </summary>
    [HttpGet("products/{productId}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductDto>> GetProductDetail(int productId)
    {
        try
        {
            var result = await _storeService.GetProductAsync(productId);
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
    /// 新增商品到購物車
    /// </summary>
    [HttpPost("cart/items")]
    public async Task<ActionResult<ShoppingCartDto>> AddToCart([FromBody] AddToCartRequestDto request)
    {
        try
        {
            var userId = GetCurrentUserId();
                    var result = await _storeService.AddToCartAsync(userId, request.ProductId, request.Quantity);
        if (result)
        {
            var cart = await _storeService.GetShoppingCartAsync(userId);
            return Ok(cart);
        }
        return BadRequest(new { message = "加入購物車失敗" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "新增商品到購物車時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取購物車
    /// </summary>
    [HttpGet("cart")]
    public async Task<ActionResult<ShoppingCartDto>> GetCart()
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _storeService.GetShoppingCartAsync(userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取購物車時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 更新購物車商品數量
    /// </summary>
    [HttpPut("cart/items/{productId}")]
    public async Task<ActionResult<ShoppingCartDto>> UpdateCartItem(int productId, [FromBody] UpdateCartItemRequestDto request)
    {
        try
        {
            var userId = GetCurrentUserId();
                    var result = await _storeService.UpdateCartQuantityAsync(userId, productId, request.Quantity);
        if (result)
        {
            var cart = await _storeService.GetShoppingCartAsync(userId);
            return Ok(cart);
        }
        return BadRequest(new { message = "更新失敗" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新購物車商品數量時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 移除購物車商品
    /// </summary>
    [HttpDelete("cart/items/{productId}")]
    public async Task<ActionResult<ShoppingCartDto>> RemoveFromCart(int productId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _storeService.RemoveFromCartAsync(userId, productId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "移除購物車商品時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 清空購物車
    /// </summary>
    [HttpDelete("cart")]
    public async Task<ActionResult> ClearCart()
    {
        try
        {
            var userId = GetCurrentUserId();
            await _storeService.ClearCartAsync(userId);
            return Ok(new { message = "購物車已清空" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "清空購物車時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 建立訂單
    /// </summary>
    [HttpPost("orders")]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderRequestDto request)
    {
        try
        {
                    var userId = GetCurrentUserId();
        // Note: CreateOrderRequestDto doesn't have UserId, the service should get it from context
        var result = await _storeService.CreateOrderAsync(request);
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
            _logger.LogError(ex, "建立訂單時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取訂單詳情
    /// </summary>
    [HttpGet("orders/{orderId}")]
    public async Task<ActionResult<OrderDto>> GetOrderDetail(int orderId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _storeService.GetOrderAsync(orderId, userId);
            if (result == null)
                return NotFound(new { message = "訂單不存在" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取訂單詳情時發生錯誤: {OrderId}", orderId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取用戶訂單列表
    /// </summary>
    [HttpGet("orders")]
    public async Task<ActionResult<PagedOrdersDto>> GetOrders([FromQuery] OrderSearchQueryDto query)
    {
        try
        {
            var userId = GetCurrentUserId();
            query.UserId = userId; // 確保只能查看自己的訂單
            var result = await _storeService.GetUserOrdersAsync(userId, query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取訂單列表時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 付款回調
    /// </summary>
    [HttpPost("payment/callback")]
    [AllowAnonymous]
    public async Task<ActionResult> PaymentCallback([FromBody] PaymentCallbackDto callback)
    {
        try
        {
            var result = await _storeService.ProcessPaymentCallbackAsync(callback);
            if (result)
                return Ok(new { message = "付款處理成功" });
            else
                return BadRequest(new { message = "付款處理失敗" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "付款回調處理時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取商城排行榜
    /// </summary>
    [HttpGet("rankings")]
    [AllowAnonymous]
    public async Task<ActionResult<List<StoreRankingDto>>> GetStoreRankings([FromQuery] string period = "week", [FromQuery] int limit = 10)
    {
        try
        {
            var rankingQuery = new RankingQueryDto { PeriodType = period };
        var result = await _storeService.GetStoreRankingsAsync(rankingQuery);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取商城排行榜時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 搜尋商品
    /// </summary>
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedProductsDto>> SearchProducts([FromQuery] string keyword, [FromQuery] ProductSearchQueryDto query)
    {
        try
        {
            query.Keyword = keyword;
            var result = await _storeService.SearchProductsAsync(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "搜尋商品時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取商品類別
    /// </summary>
    [HttpGet("categories")]
    [AllowAnonymous]
    public async Task<ActionResult<List<SupplierDto>>> GetCategories()
    {
        try
        {
            var result = await _storeService.GetSuppliersAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取商品類別時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取供應商列表
    /// </summary>
    [HttpGet("suppliers")]
    [AllowAnonymous]
    public async Task<ActionResult<List<SupplierDto>>> GetSuppliers()
    {
        try
        {
            var result = await _storeService.GetSuppliersAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取供應商列表時發生錯誤");
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
/// 新增到購物車請求 DTO
/// </summary>
public class AddToCartRequestDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

/// <summary>
/// 更新購物車商品請求 DTO
/// </summary>
public class UpdateCartItemRequestDto
{
    public int Quantity { get; set; }
}

/// <summary>
/// 退款請求 DTO
/// </summary>
public class RefundRequestDto
{
    public string Reason { get; set; } = string.Empty;
    public string? Description { get; set; }
}