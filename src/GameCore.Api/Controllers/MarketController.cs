using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GameCore.Api.Services;
using GameCore.Shared.DTOs;
using GameCore.Shared.Interfaces;
using System.Security.Claims;

namespace GameCore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
        /// 上架商品
        /// </summary>
        [HttpPost("list")]
        public async Task<IActionResult> ListItem([FromBody] ListItemRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "未授權" });
                }

                request.SellerId = userId.Value;
                var result = await _marketService.ListItemAsync(request);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "上架商品失敗");
                return StatusCode(500, new { message = "內部伺服器錯誤" });
            }
        }

        /// <summary>
        /// 購買商品
        /// </summary>
        [HttpPost("buy")]
        public async Task<IActionResult> BuyItem([FromBody] BuyItemRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "未授權" });
                }

                request.BuyerId = userId.Value;
                var result = await _marketService.BuyItemAsync(request);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "購買商品失敗");
                return StatusCode(500, new { message = "內部伺服器錯誤" });
            }
        }

        /// <summary>
        /// 搜尋商品
        /// </summary>
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchItems([FromQuery] MarketSearchRequest request)
        {
            try
            {
                var result = await _marketService.GetMarketItemsAsync(request);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "搜尋商品失敗");
                return StatusCode(500, new { message = "內部伺服器錯誤" });
            }
        }

        /// <summary>
        /// 獲取用戶交易記錄
        /// </summary>
        [HttpGet("transactions")]
        public async Task<IActionResult> GetUserTransactions([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "未授權" });
                }

                var result = await _marketService.GetUserTransactionsAsync(userId.Value, page, pageSize);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取交易記錄失敗");
                return StatusCode(500, new { message = "內部伺服器錯誤" });
            }
        }

        /// <summary>
        /// 獲取市場統計
        /// </summary>
        [HttpGet("statistics")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMarketStatistics()
        {
            try
            {
                var result = await _marketService.GetMarketStatisticsAsync();

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取市場統計失敗");
                return StatusCode(500, new { message = "內部伺服器錯誤" });
            }
        }

        /// <summary>
        /// 取消商品
        /// </summary>
        [HttpDelete("{marketItemId}")]
        public async Task<IActionResult> CancelItem(int marketItemId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "未授權" });
                }

                var result = await _marketService.CancelMarketItemAsync(marketItemId, userId.Value);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取消商品失敗");
                return StatusCode(500, new { message = "內部伺服器錯誤" });
            }
        }

        /// <summary>
        /// 更新商品
        /// </summary>
        [HttpPut("{marketItemId}")]
        public async Task<IActionResult> UpdateItem(int marketItemId, [FromBody] UpdateItemRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "未授權" });
                }

                var result = await _marketService.UpdateItemAsync(marketItemId, userId.Value, request.Price, request.Description);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新商品失敗");
                return StatusCode(500, new { message = "內部伺服器錯誤" });
            }
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return null;
        }
    }

    public class UpdateItemRequest
    {
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
