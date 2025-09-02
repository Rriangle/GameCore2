using GameCore.Api.Services;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameCore.Api.Controllers;

/// <summary>
/// 錢包控制器
/// 處理錢包相關的 HTTP 請求
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    /// <summary>
    /// 取得使用者錢包資訊
    /// </summary>
    /// <returns>錢包資訊</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<WalletDto>>> GetWallet()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new ApiResponse<WalletDto> { Success = false, Message = "未授權" });

        var wallet = await _walletService.GetWalletAsync(userId.Value);
        if (wallet == null)
            return NotFound(new ApiResponse<WalletDto> { Success = false, Message = "錢包不存在" });

        return Ok(new ApiResponse<WalletDto>
        {
            Success = true,
            Message = "取得錢包資訊成功",
            Data = wallet
        });
    }

    /// <summary>
    /// 增加點數
    /// </summary>
    /// <param name="request">增加點數請求</param>
    /// <returns>操作結果</returns>
    [HttpPost("add-points")]
    public async Task<ActionResult<ApiResponse<bool>>> AddPoints([FromBody] AddPointsRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new ApiResponse<bool> { Success = false, Message = "未授權" });

        var result = await _walletService.AddPointsAsync(userId.Value, request.Points, request.Reason);
        
        if (!result.Success)
            return BadRequest(new ApiResponse<bool> { Success = false, Message = result.Message });

        return Ok(new ApiResponse<bool>
        {
            Success = true,
            Message = "增加點數成功",
            Data = true
        });
    }

    /// <summary>
    /// 扣除點數
    /// </summary>
    /// <param name="request">扣除點數請求</param>
    /// <returns>操作結果</returns>
    [HttpPost("deduct-points")]
    public async Task<ActionResult<ApiResponse<bool>>> DeductPoints([FromBody] DeductPointsRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new ApiResponse<bool> { Success = false, Message = "未授權" });

        var result = await _walletService.DeductPointsAsync(userId.Value, request.Points, request.Reason);
        
        if (!result.Success)
            return BadRequest(new ApiResponse<bool> { Success = false, Message = result.Message });

        return Ok(new ApiResponse<bool>
        {
            Success = true,
            Message = "扣除點數成功",
            Data = true
        });
    }

    /// <summary>
    /// 轉帳給其他使用者
    /// </summary>
    /// <param name="request">轉帳請求</param>
    /// <returns>操作結果</returns>
    [HttpPost("transfer")]
    public async Task<ActionResult<ApiResponse<bool>>> TransferPoints([FromBody] TransferPointsRequest request)
    {
        var fromUserId = GetCurrentUserId();
        if (fromUserId == null)
            return Unauthorized(new ApiResponse<bool> { Success = false, Message = "未授權" });

        var result = await _walletService.TransferPointsAsync(fromUserId.Value, request.ToUserId, request.Points, request.Reason);
        
        if (!result.Success)
            return BadRequest(new ApiResponse<bool> { Success = false, Message = result.Message });

        return Ok(new ApiResponse<bool>
        {
            Success = true,
            Message = "轉帳成功",
            Data = true
        });
    }

    /// <summary>
    /// 取得交易歷史
    /// </summary>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>交易歷史</returns>
    [HttpGet("transactions")]
    public async Task<ActionResult<ApiResponse<PagedResult<TransactionDto>>>> GetTransactions(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new ApiResponse<PagedResult<TransactionDto>> { Success = false, Message = "未授權" });

        var transactions = await _walletService.GetTransactionHistoryAsync(userId.Value, page, pageSize);

        return Ok(new ApiResponse<PagedResult<TransactionDto>>
        {
            Success = true,
            Message = "取得交易歷史成功",
            Data = transactions
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

/// <summary>
/// 增加點數請求模型
/// </summary>
public class AddPointsRequest
{
    public int Points { get; set; }
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// 扣除點數請求模型
/// </summary>
public class DeductPointsRequest
{
    public int Points { get; set; }
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// 轉帳請求模型
/// </summary>
public class TransferPointsRequest
{
    public int ToUserId { get; set; }
    public int Points { get; set; }
    public string Reason { get; set; } = string.Empty;
} 