using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameCore.Mvc.Controllers;

/// <summary>
/// 銷售控制器，處理銷售相關的 API 請求
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SalesController : ControllerBase
{
    private readonly ISalesService _salesService;
    private readonly ILogger<SalesController> _logger;

    /// <summary>
    /// 建構函式，注入相依服務
    /// </summary>
    /// <param name="salesService">銷售服務</param>
    /// <param name="logger">日誌記錄器</param>
    public SalesController(ISalesService salesService, ILogger<SalesController> logger)
    {
        _salesService = salesService;
        _logger = logger;
    }

    /// <summary>
    /// 申請銷售功能
    /// </summary>
    /// <param name="application">申請資料</param>
    /// <returns>申請結果</returns>
    /// <response code="200">申請成功</response>
    /// <response code="400">申請失敗</response>
    /// <response code="401">未授權</response>
    [HttpPost("apply")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> ApplySalesProfile([FromBody] SalesProfileApplicationDto application)
    {
        try
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("用戶 {UserId} 申請銷售功能", userId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _salesService.ApplySalesProfileAsync(userId, application);
            if (!success)
            {
                return BadRequest(new { message = "申請失敗，請檢查是否已申請過或銀行帳號是否重複" });
            }

            return Ok(new { message = "銷售功能申請成功，等待審核" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "申請銷售功能時發生錯誤");
            return StatusCode(500, new { message = "伺服器內部錯誤" });
        }
    }

    /// <summary>
    /// 取得銷售資料
    /// </summary>
    /// <returns>銷售資料</returns>
    /// <response code="200">成功取得銷售資料</response>
    /// <response code="404">銷售資料不存在</response>
    /// <response code="401">未授權</response>
    [HttpGet("profile")]
    [ProducesResponseType(typeof(SalesProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SalesProfileDto>> GetSalesProfile()
    {
        try
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("用戶 {UserId} 請求查詢銷售資料", userId);

            var profile = await _salesService.GetSalesProfileAsync(userId);
            if (profile == null)
            {
                return NotFound(new { message = "銷售資料不存在" });
            }

            return Ok(profile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得銷售資料時發生錯誤");
            return StatusCode(500, new { message = "伺服器內部錯誤" });
        }
    }

    /// <summary>
    /// 更新銷售資料
    /// </summary>
    /// <param name="application">更新資料</param>
    /// <returns>更新結果</returns>
    /// <response code="200">更新成功</response>
    /// <response code="400">更新失敗</response>
    /// <response code="401">未授權</response>
    [HttpPut("profile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> UpdateSalesProfile([FromBody] SalesProfileApplicationDto application)
    {
        try
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("用戶 {UserId} 更新銷售資料", userId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _salesService.UpdateSalesProfileAsync(userId, application);
            if (!success)
            {
                return BadRequest(new { message = "更新失敗，請檢查銷售資料是否存在或銀行帳號是否重複" });
            }

            return Ok(new { message = "銷售資料更新成功" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新銷售資料時發生錯誤");
            return StatusCode(500, new { message = "伺服器內部錯誤" });
        }
    }

    /// <summary>
    /// 取得銷售錢包資訊
    /// </summary>
    /// <returns>銷售錢包資訊</returns>
    /// <response code="200">成功取得銷售錢包</response>
    /// <response code="404">銷售錢包不存在</response>
    /// <response code="401">未授權</response>
    [HttpGet("wallet")]
    [ProducesResponseType(typeof(SalesWalletDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SalesWalletDto>> GetSalesWallet()
    {
        try
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("用戶 {UserId} 請求查詢銷售錢包", userId);

            var wallet = await _salesService.GetSalesWalletAsync(userId);
            if (wallet == null)
            {
                return NotFound(new { message = "銷售錢包不存在" });
            }

            return Ok(wallet);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得銷售錢包時發生錯誤");
            return StatusCode(500, new { message = "伺服器內部錯誤" });
        }
    }

    /// <summary>
    /// 檢查用戶是否有銷售權限
    /// </summary>
    /// <returns>銷售權限狀態</returns>
    /// <response code="200">成功檢查銷售權限</response>
    /// <response code="401">未授權</response>
    [HttpGet("authority")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> CheckSalesAuthority()
    {
        try
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("用戶 {UserId} 請求檢查銷售權限", userId);

            var hasAuthority = await _salesService.HasSalesAuthorityAsync(userId);
            return Ok(new { hasSalesAuthority = hasAuthority });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查銷售權限時發生錯誤");
            return StatusCode(500, new { message = "伺服器內部錯誤" });
        }
    }

    /// <summary>
    /// 檢查銷售錢包餘額是否足夠
    /// </summary>
    /// <param name="requiredAmount">需要的金額</param>
    /// <returns>餘額檢查結果</returns>
    /// <response code="200">成功檢查餘額</response>
    /// <response code="400">參數錯誤</response>
    /// <response code="401">未授權</response>
    [HttpGet("check-balance/{requiredAmount:int}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> CheckSalesBalance(int requiredAmount)
    {
        try
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("用戶 {UserId} 請求檢查銷售錢包餘額是否足夠 {RequiredAmount}", userId, requiredAmount);

            if (requiredAmount < 0)
            {
                return BadRequest(new { message = "金額不能為負數" });
            }

            var hasSufficient = await _salesService.HasSufficientSalesBalanceAsync(userId, requiredAmount);
            var wallet = await _salesService.GetSalesWalletAsync(userId);

            return Ok(new
            {
                hasSufficientBalance = hasSufficient,
                currentBalance = wallet?.UserSales_Wallet ?? 0,
                requiredAmount = requiredAmount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查銷售錢包餘額時發生錯誤");
            return StatusCode(500, new { message = "伺服器內部錯誤" });
        }
    }

    /// <summary>
    /// 取得待審核的銷售申請列表 (管理者用)
    /// </summary>
    /// <returns>待審核申請列表</returns>
    /// <response code="200">成功取得待審核申請</response>
    /// <response code="401">未授權</response>
    /// <response code="403">權限不足</response>
    [HttpGet("admin/pending-applications")]
    [ProducesResponseType(typeof(List<SalesProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<SalesProfileDto>>> GetPendingApplications()
    {
        try
        {
            var adminUserId = GetCurrentUserId();
            _logger.LogInformation("管理者 {AdminUserId} 請求查詢待審核的銷售申請", adminUserId);

            // TODO: 檢查管理者權限
            // if (!await _authService.IsAdminAsync(adminUserId))
            // {
            //     return Forbid("權限不足");
            // }

            var applications = await _salesService.GetPendingApplicationsAsync();
            return Ok(applications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得待審核銷售申請時發生錯誤");
            return StatusCode(500, new { message = "伺服器內部錯誤" });
        }
    }

    /// <summary>
    /// 管理者審核銷售申請
    /// </summary>
    /// <param name="userId">申請用戶編號</param>
    /// <param name="request">審核請求</param>
    /// <returns>審核結果</returns>
    /// <response code="200">審核成功</response>
    /// <response code="400">審核失敗</response>
    /// <response code="401">未授權</response>
    /// <response code="403">權限不足</response>
    [HttpPost("admin/review/{userId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> ReviewSalesApplication(int userId, [FromBody] SalesApplicationReviewDto request)
    {
        try
        {
            var adminUserId = GetCurrentUserId();
            _logger.LogInformation("管理者 {AdminUserId} 審核用戶 {UserId} 的銷售申請，結果: {Approved}", 
                adminUserId, userId, request.Approved);

            // TODO: 檢查管理者權限
            // if (!await _authService.IsAdminAsync(adminUserId))
            // {
            //     return Forbid("權限不足");
            // }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _salesService.ReviewSalesApplicationAsync(adminUserId, userId, request.Approved, request.Reason);
            if (!success)
            {
                return BadRequest(new { message = "審核失敗，請檢查申請是否存在" });
            }

            var resultMessage = request.Approved ? "銷售申請已核准" : "銷售申請已拒絕";
            return Ok(new { message = resultMessage, approved = request.Approved, reason = request.Reason });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "審核銷售申請時發生錯誤");
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

/// <summary>
/// 銷售申請審核請求 DTO
/// </summary>
public class SalesApplicationReviewDto
{
    /// <summary>
    /// 是否核准
    /// </summary>
    public bool Approved { get; set; }

    /// <summary>
    /// 審核理由
    /// </summary>
    public string Reason { get; set; } = string.Empty;
}