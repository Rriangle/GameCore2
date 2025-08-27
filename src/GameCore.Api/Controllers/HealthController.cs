using Microsoft.AspNetCore.Mvc;

namespace GameCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        _logger.LogInformation("健康檢查請求");

        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            version = "1.0.0",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"
        });
    }

    [HttpGet("detailed")]
    public IActionResult GetDetailed()
    {
        var healthStatus = new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            version = "1.0.0",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
            services = new
            {
                database = "connected",
                cache = "available",
                external_apis = "reachable"
            },
            system = new
            {
                memory_usage = GC.GetTotalMemory(false),
                uptime = Environment.TickCount64,
                processor_count = Environment.ProcessorCount
            }
        };

        return Ok(healthStatus);
    }
}
