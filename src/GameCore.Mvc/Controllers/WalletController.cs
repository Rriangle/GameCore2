using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameCore.Mvc.Controllers;

/// <summary>
/// 錢包控制器，處理錢包相關的 API 請求
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;
    private readonly ILogger<WalletController> _logger;

    /// <summary>
    /// 建構函式，注入相依服務
    /// </summary>
    /// <param name="walletService">錢包服務</param>
    /// <param name="logger">日誌記錄器</param>
    public WalletController(IWalletService walletService, ILogger<WalletController> logger)
    {
        _walletService = walletService;
        _logger = logger;
    }

    /// <summary>
    /// 取得用戶錢包餘額
    /// </summary>
    /// <returns>錢包餘額資訊</returns>
    /// <response code="200">成功取得錢包餘額</response>
    /// <response code="404">錢包不存在</response>
    /// <response code="401">未授權</response>
    [HttpGet("balance")]
    [ProducesResponseType(typeof(WalletBalanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<WalletBalanceDto>> GetWalletBalance()
    {
        try
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("用戶 {UserId} 請求查詢錢包餘額", userId);

            var balance = await _walletService.GetWalletBalanceAsync(userId);
            if (balance == null)
            {
                _logger.LogWarning("用戶 {UserId} 的錢包不存在", userId);
                return NotFound(new { message = "錢包不存在" });
            }

            return Ok(balance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得錢包餘額時發生錯誤");
            return StatusCode(500, new { message = "伺服器內部錯誤" });
        }
    }

    /// <summary>
    /// 取得點數交易明細 (分頁查詢)
    /// </summary>
    /// <param name="query">查詢條件</param>
    /// <returns>分頁交易明細</returns>
    /// <response code="200">成功取得交易明細</response>
    /// <response code="400">查詢參數錯誤</response>
    /// <response code="401">未授權</response>
    [HttpGet("transactions")]
    [ProducesResponseType(typeof(PagedPointTransactionsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedPointTransactionsDto>> GetPointTransactions([FromQuery] PointTransactionQueryDto query)
    {
        try
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("用戶 {UserId} 請求查詢點數交易明細", userId);

            if (query.Page < 1)
                query.Page = 1;
            if (query.PageSize < 1 || query.PageSize > 100)
                query.PageSize = 20;

            var transactions = await _walletService.GetPointTransactionsAsync(userId, query);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得點數交易明細時發生錯誤");
            return StatusCode(500, new { message = "伺服器內部錯誤" });
        }
    }

    /// <summary>
    /// 檢查用戶是否有足夠點數
    /// </summary>
    /// <param name="requiredPoints">需要的點數</param>
    /// <returns>是否有足夠點數</returns>
    /// <response code="200">成功檢查點數</response>
    /// <response code="400">參數錯誤</response>
    /// <response code="401">未授權</response>
    [HttpGet("check-balance/{requiredPoints:int}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> CheckSufficientPoints(int requiredPoints)
    {
        try
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("用戶 {UserId} 請求檢查是否有足夠點數 {RequiredPoints}", userId, requiredPoints);

            if (requiredPoints < 0)
            {
                return BadRequest(new { message = "點數不能為負數" });
            }

            var hasSufficient = await _walletService.HasSufficientPointsAsync(userId, requiredPoints);
            var currentBalance = await _walletService.GetWalletBalanceAsync(userId);

            return Ok(new
            {
                hasSufficientPoints = hasSufficient,
                currentBalance = currentBalance?.Balance ?? 0,
                requiredPoints = requiredPoints
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查點數時發生錯誤");
            return StatusCode(500, new { message = "伺服器內部錯誤" });
        }
    }

    /// <summary>
    /// 取得銀行代號清單
    /// </summary>
    /// <returns>銀行代號清單</returns>
    /// <response code="200">成功取得銀行代號清單</response>
    [HttpGet("bank-codes")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<BankCodeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<BankCodeDto>>> GetBankCodes()
    {
        try
        {
            _logger.LogInformation("請求取得銀行代號清單");

            var bankCodes = await _walletService.GetBankCodesAsync();
            return Ok(bankCodes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得銀行代號清單時發生錯誤");
            return StatusCode(500, new { message = "伺服器內部錯誤" });
        }
    }

    /// <summary>
    /// 管理者調整用戶點數
    /// </summary>
    /// <param name="request">調整請求</param>
    /// <returns>調整結果</returns>
    /// <response code="200">成功調整點數</response>
    /// <response code="400">請求參數錯誤</response>
    /// <response code="401">未授權</response>
    /// <response code="403">權限不足</response>
    [HttpPost("admin/adjust-points")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> AdminAdjustPoints([FromBody] AdminAdjustPointsDto request)
    {
        try
        {
            var adminUserId = GetCurrentUserId();
            _logger.LogInformation("管理者 {AdminUserId} 請求調整用戶 {UserId} 點數: {Delta}", 
                adminUserId, request.UserId, request.Delta);

            // TODO: 檢查管理者權限
            // if (!await _authService.IsAdminAsync(adminUserId))
            // {
            //     return Forbid("權限不足");
            // }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _walletService.AdminAdjustPointsAsync(adminUserId, request);
            if (!success)
            {
                return BadRequest(new { message = "點數調整失敗，請檢查用戶是否存在或餘額是否足夠" });
            }

            return Ok(new { message = "點數調整成功", delta = request.Delta, reason = request.Reason });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "管理者調整點數時發生錯誤");
            return StatusCode(500, new { message = "伺服器內部錯誤" });
        }
    }

    /// <summary>
    /// 取得目前登入用戶的編號
    /// </summary>
    /// <returns>用戶編號</returns>
    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        throw new UnauthorizedAccessException("無法取得用戶編號");
    }
}