using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GameCore.Tests
{
    public class HealthControllerTests
    {
        [Fact]
        public void HealthCheck_ShouldReturnOk()
        {
            // Arrange
            var app = TestProgram.CreateTestApplication();
            var client = app.CreateClient();

            // Act
            var response = client.GetAsync("/health").Result;

            // Assert
            Assert.True(response.IsSuccessStatusCode);
        }
    }
}
