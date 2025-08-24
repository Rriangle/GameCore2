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
        var response = Assert.IsType<dynamic>(okResult.Value);
        
        Assert.Equal("healthy", response.status);
        Assert.NotNull(response.timestamp);
        Assert.Equal("1.0.0", response.version);
    }

    [Fact]
    public async Task GetDetailed_ShouldReturnOkResult()
    {
        // Act
        var result = await _controller.GetDetailed();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<dynamic>(okResult.Value);
        
        Assert.Equal("healthy", response.status);
        Assert.NotNull(response.timestamp);
        Assert.Equal("1.0.0", response.version);
        Assert.NotNull(response.services);
        Assert.NotNull(response.system);
    }
}
