namespace GameCore.Api.Models;

public class HealthResponse
{
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Version { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
}

public class DetailedHealthResponse : HealthResponse
{
    public HealthServices Services { get; set; } = new();
    public SystemInfo System { get; set; } = new();
}

public class HealthServices
{
    public string Database { get; set; } = string.Empty;
    public string Cache { get; set; } = string.Empty;
    public string ExternalApis { get; set; } = string.Empty;
}

public class SystemInfo
{
    public long MemoryUsage { get; set; }
    public long Uptime { get; set; }
    public int ProcessorCount { get; set; }
}