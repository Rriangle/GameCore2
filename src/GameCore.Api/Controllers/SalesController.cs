using GameCore.Api.Services;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameCore.Api.Controllers;

/// <summary>
/// 銷售控制器
/// 處理銷售相關的 HTTP 請求
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SalesController : ControllerBase
{
    private readonly ISalesService _salesService;

    public SalesController(ISalesService salesService)
    {
        _salesService = salesService;
    }

    /// <summary>
    /// 取得使用者銷售資料
    /// </summary>
    /// <returns>銷售資料</returns>
    [HttpGet("profile")]
    public async Task<ActionResult<ApiResponse<SalesProfileDto>>> GetSalesProfile()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new ApiResponse<SalesProfileDto> { Success = false, Message = "未授權" });

        var profile = await _salesService.GetSalesProfileAsync(userId.Value);
        if (profile == null)
            return NotFound(new ApiResponse<SalesProfileDto> { Success = false, Message = "銷售資料不存在" });

        return Ok(new ApiResponse<SalesProfileDto>
        {
            Success = true,
            Message = "取得銷售資料成功",
            Data = profile
        });
    }

    /// <summary>
    /// 更新銷售資料
    /// </summary>
    /// <param name="request">更新請求</param>
    /// <returns>操作結果</returns>
    [HttpPut("profile")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateSalesProfile([FromBody] UpdateSalesProfileRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new ApiResponse<bool> { Success = false, Message = "未授權" });

        var result = await _salesService.UpdateSalesProfileAsync(userId.Value, request);
        
        if (!result.Success)
            return BadRequest(new ApiResponse<bool> { Success = false, Message = result.Message });

        return Ok(new ApiResponse<bool>
        {
            Success = true,
            Message = "更新銷售資料成功",
            Data = true
        });
    }

    /// <summary>
    /// 取得銷售統計資訊
    /// </summary>
    /// <returns>銷售統計</returns>
    [HttpGet("statistics")]
    public async Task<ActionResult<ApiResponse<SalesStatisticsDto>>> GetSalesStatistics()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new ApiResponse<SalesStatisticsDto> { Success = false, Message = "未授權" });

        var statistics = await _salesService.GetSalesStatisticsAsync(userId.Value);
        if (statistics == null)
            return NotFound(new ApiResponse<SalesStatisticsDto> { Success = false, Message = "銷售統計不存在" });

        return Ok(new ApiResponse<SalesStatisticsDto>
        {
            Success = true,
            Message = "取得銷售統計成功",
            Data = statistics
        });
    }

    /// <summary>
    /// 取得銷售排行榜
    /// </summary>
    /// <param name="top">前幾名</param>
    /// <returns>銷售排行榜</returns>
    [HttpGet("ranking")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<SalesRankingDto>>>> GetSalesRanking([FromQuery] int top = 10)
    {
        if (top <= 0 || top > 100)
            return BadRequest(new ApiResponse<List<SalesRankingDto>> { Success = false, Message = "排行榜數量必須在 1-100 之間" });

        var rankings = await _salesService.GetSalesRankingAsync(top);

        return Ok(new ApiResponse<List<SalesRankingDto>>
        {
            Success = true,
            Message = "取得銷售排行榜成功",
            Data = rankings
        });
    }

    /// <summary>
    /// 計算佣金
    /// </summary>
    /// <param name="orderAmount">訂單金額</param>
    /// <returns>佣金金額</returns>
    [HttpGet("commission")]
    public async Task<ActionResult<ApiResponse<decimal>>> CalculateCommission([FromQuery] decimal orderAmount)
    {
        if (orderAmount <= 0)
            return BadRequest(new ApiResponse<decimal> { Success = false, Message = "訂單金額必須大於0" });

        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new ApiResponse<decimal> { Success = false, Message = "未授權" });

        var commission = await _salesService.CalculateCommissionAsync(userId.Value, orderAmount);

        return Ok(new ApiResponse<decimal>
        {
            Success = true,
            Message = "計算佣金成功",
            Data = commission
        });
    }

    /// <summary>
    /// 取得當前使用者ID
    /// </summary>
    /// <returns>使用者ID</returns>
    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("userId");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            return null;

        return userId;
    }
} 