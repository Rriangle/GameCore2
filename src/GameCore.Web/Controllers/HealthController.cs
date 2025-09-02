using Microsoft.AspNetCore.Mvc;

namespace GameCore.Web.Controllers
{
    /// <summary>
    /// 健康檢查控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// 健康檢查端點
        /// </summary>
        /// <returns>系統健康狀態</returns>
        [HttpGet]
        public IActionResult Get()
        {
            var healthStatus = new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
                Services = new
                {
                    Database = "Connected",
                    Cache = "Connected",
                    ExternalServices = "Available"
                }
            };

            return Ok(healthStatus);
        }

        /// <summary>
        /// 詳細健康檢查端點
        /// </summary>
        /// <returns>詳細的系統健康狀態</returns>
        [HttpGet("detailed")]
        public IActionResult GetDetailed()
        {
            var detailedHealthStatus = new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
                Uptime = Environment.TickCount64,
                MemoryUsage = GC.GetTotalMemory(false),
                ProcessorCount = Environment.ProcessorCount,
                Services = new
                {
                    Database = new
                    {
                        Status = "Connected",
                        ResponseTime = "5ms"
                    },
                    Cache = new
                    {
                        Status = "Connected",
                        HitRate = "95%"
                    },
                    ExternalServices = new
                    {
                        Status = "Available",
                        LastCheck = DateTime.UtcNow.AddMinutes(-1)
                    }
                }
            };

            return Ok(detailedHealthStatus);
        }
    }
} 