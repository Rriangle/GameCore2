using GameCore.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameCore.Tests;

public class HealthControllerTests
{
    private readonly Mock<ILogger<HealthController>> _loggerMock;
    private readonly HealthController _controller;

    public HealthControllerTests()
    {
        _loggerMock = new Mock<ILogger<HealthController>>();
        _controller = new HealthController(_loggerMock.Object);
    }

    [Fact]
    public void Get_ShouldReturnOkResult()
    {
        // Act
        var result = _controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        
        // 使用反射檢查匿名類型的屬性
        var responseType = okResult.Value.GetType();
        var statusProperty = responseType.GetProperty("status");
        var timestampProperty = responseType.GetProperty("timestamp");
        var versionProperty = responseType.GetProperty("version");
        
        Assert.NotNull(statusProperty);
        Assert.NotNull(timestampProperty);
        Assert.NotNull(versionProperty);
        
        Assert.Equal("healthy", statusProperty.GetValue(okResult.Value));
        Assert.NotNull(timestampProperty.GetValue(okResult.Value));
        Assert.Equal("1.0.0", versionProperty.GetValue(okResult.Value));
    }

    [Fact]
    public async Task GetDetailed_ShouldReturnOkResult()
    {
        // Act
        var result = await _controller.GetDetailed();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        
        // 使用反射檢查匿名類型的屬性
        var responseType = okResult.Value.GetType();
        var statusProperty = responseType.GetProperty("status");
        var timestampProperty = responseType.GetProperty("timestamp");
        var versionProperty = responseType.GetProperty("version");
        var servicesProperty = responseType.GetProperty("services");
        var systemProperty = responseType.GetProperty("system");
        
        Assert.NotNull(statusProperty);
        Assert.NotNull(timestampProperty);
        Assert.NotNull(versionProperty);
        Assert.NotNull(servicesProperty);
        Assert.NotNull(systemProperty);
        
        Assert.Equal("healthy", statusProperty.GetValue(okResult.Value));
        Assert.NotNull(timestampProperty.GetValue(okResult.Value));
        Assert.Equal("1.0.0", versionProperty.GetValue(okResult.Value));
        Assert.NotNull(servicesProperty.GetValue(okResult.Value));
        Assert.NotNull(systemProperty.GetValue(okResult.Value));
    }
}
