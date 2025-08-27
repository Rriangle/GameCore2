using GameCore.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameCore.Api.Controllers;

/// <summary>
/// 假資料控制器，用於生成測試和展示用的假資料
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // 只有管理員可以生成假資料
public class FakeDataController : ControllerBase
{
    private readonly FakeDataService _fakeDataService;
    private readonly ILogger<FakeDataController> _logger;

    public FakeDataController(FakeDataService fakeDataService, ILogger<FakeDataController> logger)
    {
        _fakeDataService = fakeDataService;
        _logger = logger;
    }

    /// <summary>
    /// 生成假用戶資料
    /// </summary>
    /// <param name="count">生成數量 (預設 1000)</param>
    /// <returns>生成結果</returns>
    [HttpPost("users")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GenerateFakeUsers([FromQuery] int count = 1000)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到生成假用戶資料請求: {CorrelationId}, Count: {Count}", correlationId, count);

        try
        {
            if (count <= 0 || count > 10000)
            {
                _logger.LogWarning("生成數量超出範圍: {CorrelationId}, Count: {Count}", correlationId, count);
                return BadRequest(ApiResponse<object>.ErrorResult("生成數量必須在 1-10000 之間"));
            }

            var users = _fakeDataService.GenerateFakeUsers(count);
            var userIntroduces = _fakeDataService.GenerateFakeUserIntroduces(users);
            var userRights = _fakeDataService.GenerateFakeUserRights(users);
            var userWallets = _fakeDataService.GenerateFakeUserWallets(users);
            var memberSalesProfiles = _fakeDataService.GenerateFakeMemberSalesProfiles(users);
            var userSalesInformation = _fakeDataService.GenerateFakeUserSalesInformation(users);

            var result = new
            {
                Users = users.Count,
                UserIntroduces = userIntroduces.Count,
                UserRights = userRights.Count,
                UserWallets = userWallets.Count,
                MemberSalesProfiles = memberSalesProfiles.Count,
                UserSalesInformation = userSalesInformation.Count,
                TotalRecords = users.Count + userIntroduces.Count + userRights.Count + userWallets.Count + memberSalesProfiles.Count + userSalesInformation.Count
            };

            _logger.LogInformation("成功生成假資料: {CorrelationId}, Result: {@Result}", correlationId, result);
            return Ok(ApiResponse<object>.SuccessResult(result, $"成功生成 {count} 筆假用戶資料"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成假用戶資料時發生錯誤: {CorrelationId}", correlationId);
            return StatusCode(500, ApiResponse<object>.ErrorResult("生成假資料時發生錯誤"));
        }
    }

    /// <summary>
    /// 生成假資料統計資訊
    /// </summary>
    /// <returns>統計資訊</returns>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetFakeDataStats()
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取假資料統計資訊請求: {CorrelationId}", correlationId);

        try
        {
            // 這裡可以從資料庫獲取實際的統計資訊
            var stats = new
            {
                TotalUsers = 1000,
                ActiveUsers = 950,
                UsersWithSalesPermission = 200,
                AverageWalletBalance = 5000,
                AverageSalesWalletBalance = 25000,
                DataGeneratedAt = DateTime.UtcNow
            };

            _logger.LogInformation("成功獲取假資料統計資訊: {CorrelationId}", correlationId);
            return Ok(ApiResponse<object>.SuccessResult(stats));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取假資料統計資訊時發生錯誤: {CorrelationId}", correlationId);
            return StatusCode(500, ApiResponse<object>.ErrorResult("獲取統計資訊時發生錯誤"));
        }
    }

    /// <summary>
    /// 清理假資料
    /// </summary>
    /// <returns>清理結果</returns>
    [HttpDelete("cleanup")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> CleanupFakeData()
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到清理假資料請求: {CorrelationId}", correlationId);

        try
        {
            // 這裡可以實作清理假資料的邏輯
            var result = new
            {
                Message = "假資料清理功能尚未實作",
                CleanupAt = DateTime.UtcNow
            };

            _logger.LogInformation("假資料清理請求處理完成: {CorrelationId}", correlationId);
            return Ok(ApiResponse<object>.SuccessResult(result, "假資料清理請求已處理"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "清理假資料時發生錯誤: {CorrelationId}", correlationId);
            return StatusCode(500, ApiResponse<object>.ErrorResult("清理假資料時發生錯誤"));
        }
    }
}