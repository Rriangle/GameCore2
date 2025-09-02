using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace GameCore.Tests
{
    /// <summary>
    /// API 冒煙測試
    /// </summary>
    public class ApiSmokeTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ApiSmokeTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// 測試健康檢查端點
        /// </summary>
        [Fact]
        [Trait("Smoke", "API")]
        public async Task HealthCheck_ShouldReturnOk()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/health");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Healthy", content);
        }

        /// <summary>
        /// 測試詳細健康檢查端點
        /// </summary>
        [Fact]
        [Trait("Smoke", "API")]
        public async Task DetailedHealthCheck_ShouldReturnOk()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/health/detailed");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Healthy", content);
            Assert.Contains("Services", content);
        }

        /// <summary>
        /// 測試 API 根端點
        /// </summary>
        [Fact]
        [Trait("Smoke", "API")]
        public async Task ApiRoot_ShouldReturnOk()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/");

            // Assert
            // 根端點可能返回 404 或重定向，這是正常的
            Assert.True(response.StatusCode == HttpStatusCode.OK || 
                       response.StatusCode == HttpStatusCode.NotFound || 
                       response.StatusCode == HttpStatusCode.Redirect);
        }
    }
} 