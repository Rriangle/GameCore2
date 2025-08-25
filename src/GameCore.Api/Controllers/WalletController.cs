using GameCore.Api.DTOs;
using GameCore.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameCore.Api.Controllers;

/// <summary>
/// 錢包控制器，處理用戶點數和銷售錢包相關功能
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;
    private readonly ILogger<WalletController> _logger;

    public WalletController(IWalletService walletService, ILogger<WalletController> logger)
    {
        _walletService = walletService;
        _logger = logger;
    }

    /// <summary>
    /// 取得用戶錢包餘額
    /// </summary>
    /// <returns>錢包餘額</returns>
    [HttpGet("balance")]
    [ProducesResponseType(typeof(ApiResponse<WalletBalanceDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetBalance()
    {
        var correlationId = HttpContext.GetCorrelationId();
        var userId = GetCurrentUserId();
        
        _logger.LogInformation("收到獲取錢包餘額請求: {CorrelationId}, UserId: {UserId}", correlationId, userId);

        try
        {
            var balance = await _walletService.GetWalletBalanceAsync(userId);
            
            if (balance == null)
            {
                _logger.LogWarning("錢包不存在: {CorrelationId}, UserId: {UserId}", correlationId, userId);
                return NotFound(ApiResponse<object>.ErrorResult("錢包不存在"));
            }

            _logger.LogInformation("成功獲取錢包餘額: {CorrelationId}, UserId: {UserId}, Balance: {Balance}", 
                correlationId, userId, balance.User_Point);
            return Ok(ApiResponse<WalletBalanceDto>.SuccessResult(balance));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取錢包餘額時發生錯誤: {CorrelationId}, UserId: {UserId}", correlationId, userId);
            return StatusCode(500, ApiResponse<object>.ErrorResult("內部伺服器錯誤"));
        }
    }

    /// <summary>
    /// 取得錢包交易記錄
    /// </summary>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>交易記錄</returns>
    [HttpGet("transactions")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<WalletTransactionDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetTransactions([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var correlationId = HttpContext.GetCorrelationId();
        var userId = GetCurrentUserId();
        
        _logger.LogInformation("收到獲取錢包交易記錄請求: {CorrelationId}, UserId: {UserId}, Page: {Page}, PageSize: {PageSize}", 
            correlationId, userId, page, pageSize);

        try
        {
            var transactions = await _walletService.GetWalletTransactionsAsync(userId, page, pageSize);
            
            _logger.LogInformation("成功獲取錢包交易記錄: {CorrelationId}, UserId: {UserId}, Count: {Count}", 
                correlationId, userId, transactions.Count());
            return Ok(ApiResponse<IEnumerable<WalletTransactionDto>>.SuccessResult(transactions));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取錢包交易記錄時發生錯誤: {CorrelationId}, UserId: {UserId}", correlationId, userId);
            return StatusCode(500, ApiResponse<object>.ErrorResult("內部伺服器錯誤"));
        }
    }

    /// <summary>
    /// 取得銷售錢包餘額
    /// </summary>
    /// <returns>銷售錢包餘額</returns>
    [HttpGet("sales-balance")]
    [ProducesResponseType(typeof(ApiResponse<SalesWalletDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetSalesBalance()
    {
        var correlationId = HttpContext.GetCorrelationId();
        var userId = GetCurrentUserId();
        
        _logger.LogInformation("收到獲取銷售錢包餘額請求: {CorrelationId}, UserId: {UserId}", correlationId, userId);

        try
        {
            var balance = await _walletService.GetSalesWalletBalanceAsync(userId);
            
            if (balance == null)
            {
                _logger.LogWarning("銷售錢包不存在: {CorrelationId}, UserId: {UserId}", correlationId, userId);
                return NotFound(ApiResponse<object>.ErrorResult("銷售錢包不存在"));
            }

            _logger.LogInformation("成功獲取銷售錢包餘額: {CorrelationId}, UserId: {UserId}, Balance: {Balance}", 
                correlationId, userId, balance.UserSales_Wallet);
            return Ok(ApiResponse<SalesWalletDto>.SuccessResult(balance));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取銷售錢包餘額時發生錯誤: {CorrelationId}, UserId: {UserId}", correlationId, userId);
            return StatusCode(500, ApiResponse<object>.ErrorResult("內部伺服器錯誤"));
        }
    }

    /// <summary>
    /// 取得銷售錢包交易記錄
    /// </summary>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>交易記錄</returns>
    [HttpGet("sales-transactions")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SalesWalletTransactionDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetSalesTransactions([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var correlationId = HttpContext.GetCorrelationId();
        var userId = GetCurrentUserId();
        
        _logger.LogInformation("收到獲取銷售錢包交易記錄請求: {CorrelationId}, UserId: {UserId}, Page: {Page}, PageSize: {PageSize}", 
            correlationId, userId, page, pageSize);

        try
        {
            var transactions = await _walletService.GetSalesWalletTransactionsAsync(userId, page, pageSize);
            
            _logger.LogInformation("成功獲取銷售錢包交易記錄: {CorrelationId}, UserId: {UserId}, Count: {Count}", 
                correlationId, userId, transactions.Count());
            return Ok(ApiResponse<IEnumerable<SalesWalletTransactionDto>>.SuccessResult(transactions));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取銷售錢包交易記錄時發生錯誤: {CorrelationId}, UserId: {UserId}", correlationId, userId);
            return StatusCode(500, ApiResponse<object>.ErrorResult("內部伺服器錯誤"));
        }
    }

    /// <summary>
    /// 取得當前用戶 ID
    /// </summary>
    /// <returns>用戶 ID</returns>
    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("無效的認證資訊");
        }
        return userId;
    }
}