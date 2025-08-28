using GameCore.Api.Controllers;
using GameCore.Api.Models;
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
        var response = Assert.IsType<HealthResponse>(okResult.Value);

        Assert.Equal("healthy", response.Status);
        Assert.NotEqual(default(DateTime), response.Timestamp);
        Assert.Equal("1.0.0", response.Version);
    }

    [Fact]
    public void GetDetailed_ShouldReturnOkResult()
    {
        // Act
        var result = _controller.GetDetailed();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<DetailedHealthResponse>(okResult.Value);

        Assert.Equal("healthy", response.Status);
        Assert.NotEqual(default(DateTime), response.Timestamp);
        Assert.Equal("1.0.0", response.Version);
        Assert.NotNull(response.Services);
        Assert.NotNull(response.System);
    }
}
