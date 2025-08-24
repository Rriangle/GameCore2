using GameCore.Api.DTOs;
using GameCore.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace GameCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    /// <param name="category">商品分類</param>
    /// <returns>商品列表</returns>
    [HttpGet("products")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<StoreProductDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetProducts([FromQuery] string? category)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取商品列表請求: {CorrelationId}, Category: {Category}", correlationId, category);

        var products = await _storeService.GetProductsAsync(category);

        _logger.LogInformation("成功獲取商品列表: {CorrelationId}, Count: {Count}", correlationId, products.Count());
        return Ok(ApiResponse<IEnumerable<StoreProductDto>>.SuccessResult(products));
    }

    /// <summary>
    /// 獲取商品詳情
    /// </summary>
    /// <param name="productId">商品ID</param>
    /// <returns>商品詳情</returns>
    [HttpGet("products/{productId}")]
    [ProducesResponseType(typeof(ApiResponse<StoreProductDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetProduct(int productId)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取商品詳情請求: {CorrelationId}, ProductId: {ProductId}", correlationId, productId);

        var product = await _storeService.GetProductAsync(productId);
        if (product == null)
        {
            _logger.LogWarning("商品不存在: {CorrelationId}, ProductId: {ProductId}", correlationId, productId);
            return NotFound(ApiResponse<object>.ErrorResult("商品不存在"));
        }

        _logger.LogInformation("成功獲取商品詳情: {CorrelationId}, ProductId: {ProductId}", correlationId, productId);
        return Ok(ApiResponse<StoreProductDto>.SuccessResult(product));
    }

    /// <summary>
    /// 創建訂單
    /// </summary>
    /// <param name="request">訂單請求</param>
    /// <returns>訂單結果</returns>
    [HttpPost("orders")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<OrderResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到創建訂單請求: {CorrelationId}, ProductId: {ProductId}, Quantity: {Quantity}", 
            correlationId, request.ProductId, request.Quantity);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("創建訂單請求驗證失敗: {CorrelationId}, Errors: {Errors}", correlationId, string.Join(", ", errors));
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _storeService.CreateOrderAsync(userId, request.ProductId, request.Quantity, request.ShippingAddress);

        if (result.Success)
        {
            _logger.LogInformation("訂單創建成功: {CorrelationId}, UserId: {UserId}, OrderId: {OrderId}", 
                correlationId, userId, result.Order?.OrderId);
            return Ok(ApiResponse<OrderResult>.SuccessResult(result));
        }

        _logger.LogWarning("訂單創建失敗: {CorrelationId}, UserId: {UserId}, Message: {Message}", 
            correlationId, userId, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "創建訂單失敗"));
    }

    /// <summary>
    /// 獲取用戶訂單列表
    /// </summary>
    /// <returns>訂單列表</returns>
    [HttpGet("orders")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetUserOrders()
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取用戶訂單列表請求: {CorrelationId}", correlationId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var orders = await _storeService.GetUserOrdersAsync(userId);

        _logger.LogInformation("成功獲取用戶訂單列表: {CorrelationId}, UserId: {UserId}, Count: {Count}", 
            correlationId, userId, orders.Count());
        return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResult(orders));
    }

    /// <summary>
    /// 獲取訂單詳情
    /// </summary>
    /// <param name="orderId">訂單ID</param>
    /// <returns>訂單詳情</returns>
    [HttpGet("orders/{orderId}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<OrderDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetOrder(int orderId)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取訂單詳情請求: {CorrelationId}, OrderId: {OrderId}", correlationId, orderId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var order = await _storeService.GetOrderAsync(orderId);
        if (order == null)
        {
            _logger.LogWarning("訂單不存在: {CorrelationId}, OrderId: {OrderId}", correlationId, orderId);
            return NotFound(ApiResponse<object>.ErrorResult("訂單不存在"));
        }

        _logger.LogInformation("成功獲取訂單詳情: {CorrelationId}, OrderId: {OrderId}", correlationId, orderId);
        return Ok(ApiResponse<OrderDto>.SuccessResult(order));
    }

    /// <summary>
    /// 更新訂單狀態 (管理員功能)
    /// </summary>
    /// <param name="orderId">訂單ID</param>
    /// <param name="request">狀態更新請求</param>
    /// <returns>更新結果</returns>
    [HttpPut("orders/{orderId}/status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<OrderResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 403)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusRequestDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到更新訂單狀態請求: {CorrelationId}, OrderId: {OrderId}, NewStatus: {NewStatus}", 
            correlationId, orderId, request.NewStatus);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("更新訂單狀態請求驗證失敗: {CorrelationId}, Errors: {Errors}", correlationId, string.Join(", ", errors));
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        var result = await _storeService.UpdateOrderStatusAsync(orderId, request.NewStatus);

        if (result.Success)
        {
            _logger.LogInformation("訂單狀態更新成功: {CorrelationId}, OrderId: {OrderId}, NewStatus: {NewStatus}", 
                correlationId, orderId, request.NewStatus);
            return Ok(ApiResponse<OrderResult>.SuccessResult(result));
        }

        _logger.LogWarning("訂單狀態更新失敗: {CorrelationId}, OrderId: {OrderId}, Message: {Message}", 
            correlationId, orderId, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "更新訂單狀態失敗"));
    }

    /// <summary>
    /// 更新付款狀態 (管理員功能)
    /// </summary>
    /// <param name="orderId">訂單ID</param>
    /// <param name="request">付款狀態更新請求</param>
    /// <returns>更新結果</returns>
    [HttpPut("orders/{orderId}/payment")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<OrderResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 403)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> UpdatePaymentStatus(int orderId, [FromBody] UpdatePaymentStatusRequestDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到更新付款狀態請求: {CorrelationId}, OrderId: {OrderId}, NewStatus: {NewStatus}", 
            correlationId, orderId, request.NewStatus);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("更新付款狀態請求驗證失敗: {CorrelationId}, Errors: {Errors}", correlationId, string.Join(", ", errors));
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        var result = await _storeService.UpdatePaymentStatusAsync(orderId, request.NewStatus);

        if (result.Success)
        {
            _logger.LogInformation("付款狀態更新成功: {CorrelationId}, OrderId: {OrderId}, NewStatus: {NewStatus}", 
                correlationId, orderId, request.NewStatus);
            return Ok(ApiResponse<OrderResult>.SuccessResult(result));
        }

        _logger.LogWarning("付款狀態更新失敗: {CorrelationId}, OrderId: {OrderId}, Message: {Message}", 
            correlationId, orderId, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "更新付款狀態失敗"));
    }
}

public class CreateOrderRequestDto
{
    [Required(ErrorMessage = "商品ID為必填項目")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "數量為必填項目")]
    [Range(1, 100, ErrorMessage = "數量必須在 1-100 之間")]
    public int Quantity { get; set; }

    [StringLength(200, ErrorMessage = "收貨地址長度不能超過 200 個字元")]
    public string? ShippingAddress { get; set; }
}

public class UpdateOrderStatusRequestDto
{
    [Required(ErrorMessage = "新狀態為必填項目")]
    [StringLength(20, ErrorMessage = "狀態長度不能超過 20 個字元")]
    public string NewStatus { get; set; } = string.Empty; // Created, ToShip, Shipped, Completed
}

public class UpdatePaymentStatusRequestDto
{
    [Required(ErrorMessage = "新付款狀態為必填項目")]
    [StringLength(20, ErrorMessage = "付款狀態長度不能超過 20 個字元")]
    public string NewStatus { get; set; } = string.Empty; // Placed, Pending, Paid
}