using GameCore.Api.Services;
using GameCore.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GameCore.Api.Controllers;

/// <summary>
/// 玩家市場控制器
/// 處理玩家間的二手交易功能
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MarketController : ControllerBase
{
    private readonly MarketService _marketService;

    public MarketController(MarketService marketService)
    {
        _marketService = marketService;
    }

    /// <summary>
    /// 取得商品列表
    /// </summary>
    /// <param name="searchParams">搜尋參數</param>
    /// <returns>商品列表</returns>
    [HttpGet("products")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<MarketProductDto>>> GetProducts([FromQuery] MarketSearchDto searchParams)
    {
        try
        {
            var request = new MarketSearchRequest
            {
                Keyword = searchParams.SearchTerm,
                ProductType = searchParams.Category,
                MinPrice = searchParams.MinPrice,
                MaxPrice = searchParams.MaxPrice,
                Page = searchParams.Page,
                PageSize = searchParams.PageSize
            };

            var result = await _marketService.SearchProductsAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "取得商品列表失敗", error = ex.Message });
        }
    }

    /// <summary>
    /// 取得商品詳細資訊
    /// </summary>
    /// <param name="productId">商品ID</param>
    /// <returns>商品詳細資訊</returns>
    [HttpGet("products/{productId}")]
    [AllowAnonymous]
    public async Task<ActionResult<MarketProductDto>> GetProductDetail(int productId)
    {
        try
        {
            var product = await _marketService.GetProductAsync(productId);
            if (product == null)
            {
                return NotFound(new { message = "商品不存在" });
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "取得商品詳細資訊失敗", error = ex.Message });
        }
    }

    /// <summary>
    /// 建立市場訂單
    /// </summary>
    /// <param name="orderDto">訂單資訊</param>
    /// <returns>訂單建立結果</returns>
    [HttpPost("orders")]
    public async Task<ActionResult<MarketOrderDto>> CreateOrder([FromBody] CreateMarketOrderDto orderDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "使用者未認證" });
            }

            var result = await _marketService.CreateOrderAsync(orderDto, userId.Value);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return CreatedAtAction(nameof(GetOrderDetail), new { orderId = result.Data!.OrderID }, result.Data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "建立訂單失敗", error = ex.Message });
        }
    }

    /// <summary>
    /// 取得訂單詳細資訊
    /// </summary>
    /// <param name="orderId">訂單ID</param>
    /// <returns>訂單詳細資訊</returns>
    [HttpGet("orders/{orderId}")]
    public async Task<ActionResult<MarketOrderDto>> GetOrderDetail(int orderId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "使用者未認證" });
            }

            // 簡化版本：直接從服務取得訂單
            var orders = await _marketService.GetUserOrdersAsync(userId.Value, "buyer", 1, 100);
            var order = orders.Data.FirstOrDefault(o => o.OrderID == orderId);
            
            if (order == null)
            {
                return NotFound(new { message = "訂單不存在" });
            }

            return Ok(order);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "取得訂單詳細資訊失敗", error = ex.Message });
        }
    }

    /// <summary>
    /// 取得使用者訂單列表
    /// </summary>
    /// <param name="role">角色 (buyer/seller)</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>分頁的訂單列表</returns>
    [HttpGet("orders")]
    public async Task<ActionResult<PagedResult<MarketOrderDto>>> GetUserOrders(
        [FromQuery] string role = "buyer",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "使用者未認證" });
            }

            if (role != "buyer" && role != "seller")
            {
                return BadRequest(new { message = "無效的角色參數" });
            }

            var result = await _marketService.GetUserOrdersAsync(userId.Value, role, page, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "取得訂單列表失敗", error = ex.Message });
        }
    }

    /// <summary>
    /// 取得市場排行榜
    /// </summary>
    /// <param name="top">前幾名</param>
    /// <returns>排行榜列表</returns>
    [HttpGet("ranking")]
    [AllowAnonymous]
    public async Task<ActionResult<List<MarketRankingDto>>> GetMarketRanking([FromQuery] int top = 10)
    {
        try
        {
            if (top <= 0 || top > 100)
            {
                return BadRequest(new { message = "排行榜數量必須在 1-100 之間" });
            }

            var result = await _marketService.GetMarketRankingAsync(top);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "取得排行榜失敗", error = ex.Message });
        }
    }

    /// <summary>
    /// 取得商品類別列表
    /// </summary>
    /// <returns>商品類別列表</returns>
    [HttpGet("categories")]
    [AllowAnonymous]
    public async Task<ActionResult<List<string>>> GetCategories()
    {
        try
        {
            // 簡化版本：返回預定義的類別
            await Task.CompletedTask; // 存根實作，避免 CS1998 警告
            var categories = new List<string>
            {
                "遊戲",
                "硬體",
                "周邊",
                "收藏品",
                "其他"
            };

            return Ok(categories);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "取得類別列表失敗", error = ex.Message });
        }
    }

    /// <summary>
    /// 取得商品狀態列表
    /// </summary>
    /// <returns>商品狀態列表</returns>
    [HttpGet("conditions")]
    [AllowAnonymous]
    public async Task<ActionResult<List<string>>> GetConditions()
    {
        try
        {
            await Task.CompletedTask; // 存根實作，避免 CS1998 警告
            var conditions = new List<string>
            {
                "全新",
                "九成新",
                "八成新",
                "七成新",
                "六成新",
                "五成新以下"
            };

            return Ok(conditions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "取得狀態列表失敗", error = ex.Message });
        }
    }

    /// <summary>
    /// 取得當前使用者ID
    /// </summary>
    /// <returns>使用者ID</returns>
    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("UserId");
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }
        return null;
    }
} 