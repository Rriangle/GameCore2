using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GameCore.Mvc.Models;

namespace GameCore.Mvc.Controllers
{
    /// <summary>
    /// 首頁控制器
    /// 提供首頁、隱私權政策和錯誤處理功能
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 首頁
        /// 顯示 GameCore 平台的主要信息和功能介紹
        /// </summary>
        /// <returns>首頁視圖</returns>
        public IActionResult Index()
        {
            try
            {
                _logger.LogInformation("用戶訪問首頁");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "訪問首頁時發生錯誤");
                return RedirectToAction(nameof(Error));
            }
        }

        /// <summary>
        /// 隱私權政策頁面
        /// 顯示平台的隱私權保護政策
        /// </summary>
        /// <returns>隱私權政策視圖</returns>
        public IActionResult Privacy()
        {
            try
            {
                _logger.LogInformation("用戶訪問隱私權政策頁面");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "訪問隱私權政策頁面時發生錯誤");
                return RedirectToAction(nameof(Error));
            }
        }

        /// <summary>
        /// 錯誤頁面
        /// 顯示錯誤信息和處理建議
        /// </summary>
        /// <returns>錯誤視圖</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            try
            {
                var errorViewModel = new ErrorViewModel
                {
                    RequestId = HttpContext.TraceIdentifier
                };

                _logger.LogWarning("顯示錯誤頁面，請求 ID: {RequestId}", errorViewModel.RequestId);
                return View(errorViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "顯示錯誤頁面時發生錯誤");
                
                // 如果連錯誤頁面都無法顯示，返回簡單的錯誤信息
                return Content("發生嚴重錯誤，請聯絡系統管理員。", "text/plain");
            }
        }

        /// <summary>
        /// 健康檢查
        /// 提供系統健康狀態檢查
        /// </summary>
        /// <returns>健康狀態信息</returns>
        [HttpGet]
        public IActionResult Health()
        {
            try
            {
                var healthInfo = new
                {
                    Status = "Healthy",
                    Timestamp = DateTime.UtcNow,
                    Version = "1.0.0",
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
                };

                _logger.LogDebug("健康檢查請求");
                return Json(healthInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "健康檢查時發生錯誤");
                return Json(new { Status = "Unhealthy", Error = ex.Message });
            }
        }
    }
}
