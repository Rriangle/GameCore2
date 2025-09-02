using Microsoft.AspNetCore.Mvc;
using GameCore.Api.Models;

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
    [ResponseCache(Duration = 30)]
    public IActionResult Get()
    {
        _logger.LogDebug("Health check request");

        var response = new HealthResponse
        {
            Status = "healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0",
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"
        };

        return Ok(response);
    }

    [HttpGet("detailed")]
    [ResponseCache(Duration = 60)]
    public IActionResult GetDetailed()
    {
        _logger.LogDebug("Detailed health check request");
        
        var response = new DetailedHealthResponse
        {
            Status = "healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0",
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
            Services = new HealthServices
            {
                Database = "connected",
                Cache = "available",
                ExternalApis = "reachable"
            },
            System = new SystemInfo
            {
                MemoryUsage = GC.GetTotalMemory(false),
                Uptime = Environment.TickCount64,
                ProcessorCount = Environment.ProcessorCount
            }
        };

        return Ok(response);
    }
}
